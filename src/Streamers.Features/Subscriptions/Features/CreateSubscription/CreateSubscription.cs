using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Subscriptions.Models;

namespace Streamers.Features.Subscriptions.Features.CreateSubscription;

public record CreateSubscription(Guid SubscriptionPlanId, Guid PaymentMethodId)
    : IRequest<CreateSubscriptionResponse>;

public record CreateSubscriptionResponse(string ClientSecret);

public class CreateSubscriptionHandler(
    IStripeService stripeService,
    StreamerDbContext context,
    ICurrentUser currentUser
) : IRequestHandler<CreateSubscription, CreateSubscriptionResponse>
{
    public async Task<CreateSubscriptionResponse> Handle(
        CreateSubscription request,
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

        long applicationFeePercent = 5;

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
                pm.Id == request.PaymentMethodId && pm.StreamerId == currentUser.UserId
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (paymentMethod is null)
        {
            throw new Exception("Provided payment method is not valid for the current user.");
        }

        var subscriptionResponse = await stripeService.CreateSubscriptionAsync(
            stripeCustomerId,
            subscriptionPlan.StripePriceId,
            paymentMethod.StripePaymentMethodId,
            destinationAccountId,
            applicationFeePercent,
            cancellationToken
        );
        var subscription = new Subscription(
            Guid.NewGuid(),
            payerStreamer.Id,
            subscriptionPlan.Streamer.Id,
            subscriptionResponse.SubscriptionId,
            SubscriptionStatus.Incomplete,
            DateTime.UtcNow
        );

        context.Subscriptions.Add(subscription);
        await context.SaveChangesAsync(cancellationToken);
        return new CreateSubscriptionResponse(subscriptionResponse.ClientSecret);
    }
}
