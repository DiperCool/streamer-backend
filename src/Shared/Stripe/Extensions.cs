using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using streamer.ServiceDefaults;
using Stripe;

namespace Shared.Stripe;

public static class Extensions
{
    public static IServiceCollection AddStripe(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        StripeOptions stripeOptions = configuration.BindOptions<StripeOptions>();
        StripeConfiguration.ApiKey = stripeOptions.ApiKey;

        if (!string.IsNullOrEmpty(stripeOptions.ApiBase))
        {
            var stripeClient = new StripeClient(stripeOptions.ApiKey, apiBase: stripeOptions.ApiBase);
            StripeConfiguration.StripeClient = stripeClient;
        }

        services.AddSingleton<IStripeService, StripeService>();
        return services;
    }
}
