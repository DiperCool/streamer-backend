using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Subscriptions.Exceptions;
using Streamers.Features.Subscriptions.Models;
using Microsoft.Extensions.Configuration;
using streamer.ServiceDefaults;
using Streamers.Features.ApplicationSettings;

namespace Streamers.Features.Subscriptions.Features.CreateSubscription;

public record CreateSubscription(Guid SubscriptionPlanId, Guid PaymentMethodId)
    : IRequest<CreateSubscriptionResponse>;

public record CreateSubscriptionResponse(string ClientSecret);

public class CreateSubscriptionHandler(
    IStripeService stripeService,
    StreamerDbContext context,
    ICurrentUser currentUser,
    IConfiguration configuration
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
            throw new SubscriptionPlanNotFoundException();
        }

        var destinationAccountId = subscriptionPlan.Streamer.Partner.StripeAccountId;

        var applicationOptions = configuration.BindOptions<ApplicationOptions>();
        long applicationFeePercent = applicationOptions.FeePercent;

        var payerStreamer = await context
            .Streamers.Include(s => s.Customer)
            .Where(s => s.Id == currentUser.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (payerStreamer?.Customer?.StripeCustomerId is null)
        {
            throw new StripeCustomerNotFoundException();
        }
        var stripeCustomerId = payerStreamer.Customer.StripeCustomerId;

        var paymentMethod = await context
            .PaymentMethods.Where(pm =>
                pm.Id == request.PaymentMethodId && pm.StreamerId == currentUser.UserId
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (paymentMethod is null)
        {
            throw new InvalidPaymentMethodException();
        }

        var subscriptionResponse = await stripeService.CreateSubscriptionAsync(
            stripeCustomerId,
            subscriptionPlan.StripePriceId,
            paymentMethod.StripePaymentMethodId,
            destinationAccountId,
            applicationFeePercent,
            cancellationToken,
            new Dictionary<string, string>
            {
                { "payerId", payerStreamer.Id },
                { "streamerId", subscriptionPlan.Streamer.Id },
            }
        );
        var subscription = new Subscription(
            Guid.NewGuid(),
            payerStreamer.Id,
            subscriptionPlan.Streamer.Id,
            subscriptionResponse.SubscriptionId,
            SubscriptionStatus.Incomplete,
            DateTime.UtcNow,
            subscriptionPlan.Name
        );

        context.Subscriptions.Add(subscription);
        await context.SaveChangesAsync(cancellationToken);
        return new CreateSubscriptionResponse(subscriptionResponse.ClientSecret);
    }
}
