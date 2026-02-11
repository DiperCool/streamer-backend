using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SubscriptionPlans;
using Streamers.Features.SubscriptionPlans.Models;

namespace Streamers.Features.Partners.Features.OnboardingCompleted;

public record OnboardingCompletedCommandResponse();

public record OnboardingCompletedCommand(string StripeAccountId)
    : IRequest<OnboardingCompletedCommandResponse>;

public class OnboardingCompletedCommandHandler(
    StreamerDbContext context,
    IStripeService stripeService
) : IRequestHandler<OnboardingCompletedCommand, OnboardingCompletedCommandResponse>
{
    public async Task<OnboardingCompletedCommandResponse> Handle(
        OnboardingCompletedCommand request,
        CancellationToken cancellationToken
    )
    {
        var partner = await context.Partners.FirstOrDefaultAsync(
            p => p.StripeAccountId == request.StripeAccountId,
            cancellationToken
        );

        if (partner is null)
        {
            return new OnboardingCompletedCommandResponse();
        }

        partner.CompleteOnboarding();

        var streamer = await context.Streamers.FindAsync(partner.StreamerId);
        if (streamer is null)
        {
            return new OnboardingCompletedCommandResponse();
        }

        var product = await stripeService.GetProductByNameAsync(
            SubscriptionPlansConstants.SubscriptionPlanName,
            cancellationToken
        );

        if (product is null)
        {
            product = await stripeService.CreateProductAsync(
                SubscriptionPlansConstants.SubscriptionPlanProductName,
                SubscriptionPlansConstants.SubscriptionPlanPriceInCents,
                SubscriptionPlansConstants.SubscriptionPlanCurrency,
                cancellationToken
            );
        }

        var price =
            product.DefaultPrice?.UnitAmountDecimal / 100
            ?? SubscriptionPlansConstants.SubscriptionPlanPrice;

        var subscriptionPlan = new SubscriptionPlan(
            streamer.Id,
            product.Id,
            product.DefaultPriceId,
            SubscriptionPlansConstants.SubscriptionPlanName,
            price
        );

        streamer.EnableSubscriptions();

        context.SubscriptionPlans.Add(subscriptionPlan);

        await context.SaveChangesAsync(cancellationToken);
        return new OnboardingCompletedCommandResponse();
    }
}
