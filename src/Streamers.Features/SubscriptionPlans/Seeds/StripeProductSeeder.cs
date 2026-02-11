using Shared.Seeds;
using Shared.Stripe;
using Streamers.Features.SubscriptionPlans;

namespace Streamers.Features.SubscriptionPlans.Seeds;

public class StripeProductSeeder(IStripeService stripeService) : IDataSeeder
{
    public int Order => 1;

    public async Task SeedAllAsync()
    {
        var product = await stripeService.GetProductByNameAsync(
            SubscriptionPlansConstants.SubscriptionPlanProductName,
            CancellationToken.None
        );

        if (product is null)
        {
            await stripeService.CreateProductAsync(
                SubscriptionPlansConstants.SubscriptionPlanProductName,
                SubscriptionPlansConstants.SubscriptionPlanPriceInCents,
                SubscriptionPlansConstants.SubscriptionPlanCurrency,
                CancellationToken.None
            );
        }
    }
}
