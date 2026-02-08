using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SubscriptionPlans.Models;

namespace Streamers.Features.Payments.Features.OnboardingCompleted;

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
            PaymentsConstants.SubscriptionPlanName,
            cancellationToken
        );

        if (product is null)
        {
            product = await stripeService.CreateProductAsync(
                PaymentsConstants.SubscriptionPlanProductName,
                PaymentsConstants.SubscriptionPlanPriceInCents,
                PaymentsConstants.SubscriptionPlanCurrency,
                cancellationToken
            );
        }

        var price =
            product.DefaultPrice?.UnitAmountDecimal / 100
            ?? PaymentsConstants.SubscriptionPlanPrice;

        var subscriptionPlan = new SubscriptionPlan(
            streamer.Id,
            product.Id,
            product.DefaultPriceId,
            PaymentsConstants.SubscriptionPlanName,
            price
        );

        streamer.EnableSubscriptions();

        context.SubscriptionPlans.Add(subscriptionPlan);

        await context.SaveChangesAsync(cancellationToken);
        return new OnboardingCompletedCommandResponse();
    }
}
