using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Auth0.Services;

public static class Extensions
{
    public static IServiceCollection AddAuth0Apis(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<IAuth0Service, Auth0Service>();
        return services;
    }
}
