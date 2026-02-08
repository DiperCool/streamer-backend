using Shared.Seeds;
using Shared.Stripe;

namespace Streamers.Features.Payments.Seeds;

public class StripeProductSeeder(IStripeService stripeService) : IDataSeeder
{
    public int Order => 1;

    public async Task SeedAllAsync()
    {
        var product = await stripeService.GetProductByNameAsync(
            PaymentsConstants.SubscriptionPlanProductName,
            CancellationToken.None
        );

        if (product is null)
        {
            await stripeService.CreateProductAsync(
                PaymentsConstants.SubscriptionPlanProductName,
                PaymentsConstants.SubscriptionPlanPriceInCents,
                PaymentsConstants.SubscriptionPlanCurrency,
                CancellationToken.None
            );
        }
    }
}
