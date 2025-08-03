

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using streamer.ServiceDefaults.Identity;

namespace streamer.ServiceDefaults;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddDefaultAuthentication(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var identity = configuration.BindOptions<IdentityOptions>();
        
        builder.Services.AddAuthentication()
            .AddKeycloakJwtBearer(
                serviceName: "keycloak",
                realm: identity.Realm,
                configureOptions: options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Audience = identity.Audience;
                });
        builder.Services.AddAuthorizationBuilder();
        return services;
    }
}
