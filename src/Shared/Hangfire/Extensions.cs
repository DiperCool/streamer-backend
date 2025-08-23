using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Hangfire;

public static class Extensions
{
    public static IServiceCollection AddHangfire(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHangfire(hg =>
            hg.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(options =>
                    options.UseNpgsqlConnection(configuration.GetConnectionString("streamerdb"))
                )
        );
        services.AddHangfireServer();
        return services;
    }
}
