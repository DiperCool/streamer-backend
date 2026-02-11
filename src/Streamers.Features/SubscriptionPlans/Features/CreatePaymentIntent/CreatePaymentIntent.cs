using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.PaymentMethods.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.SubscriptionPlans.Features.CreatePaymentIntent;

public record CreatePaymentIntent(Guid SubscriptionPlanId, string PaymentMethodId)
    : IRequest<CreatePaymentIntentResponse>;

public record CreatePaymentIntentResponse(string ClientSecret);

public class CreatePaymentIntentHandler(
    IStripeService stripeService,
    StreamerDbContext context,
    ICurrentUser currentUser
) : IRequestHandler<CreatePaymentIntent, CreatePaymentIntentResponse>
{
    public async Task<CreatePaymentIntentResponse> Handle(
        CreatePaymentIntent request,
        CancellationToken cancellationToken
    )
    {
        var subscriptionPlan = await context
            .SubscriptionPlans.Include(sp => sp.Streamer)
            .ThenInclude(s => s.Partner)
            .Where(x => x.Id == request.SubscriptionPlanId)
            .FirstOrDefaultAsync(cancellationToken);

        if (subscriptionPlan is null)
        {
            throw new Exception("Subscription plan not found.");
        }

        var destinationAccountId = subscriptionPlan.Streamer.Partner.StripeAccountId;

        long applicationFeeAmount = (long)(subscriptionPlan.Price * 100 * 0.1m); // TODO: Make fee percentage configurable

        var payerStreamer = await context
            .Streamers.Include(s => s.Customer)
            .Where(s => s.Id == currentUser.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (payerStreamer?.Customer?.StripeCustomerId is null)
        {
            throw new Exception("Stripe customer not found for the current user (payer)."); // TODO: Handle this gracefully
        }
        var stripeCustomerId = payerStreamer.Customer.StripeCustomerId;

        var paymentMethod = await context
            .PaymentMethods.Where(pm =>
                pm.StripePaymentMethodId == request.PaymentMethodId
                && pm.StreamerId == currentUser.UserId
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (paymentMethod is null)
        {
            throw new Exception("Provided payment method is not valid for the current user.");
        }
        var paymentIntent = await stripeService.CreatePaymentIntentAsync(
            (long)(subscriptionPlan.Price * 100),
            "usd",
            stripeCustomerId,
            request.PaymentMethodId,
            destinationAccountId,
            applicationFeeAmount,
            cancellationToken
        );

        return new CreatePaymentIntentResponse(paymentIntent.ClientSecret);
    }
}
