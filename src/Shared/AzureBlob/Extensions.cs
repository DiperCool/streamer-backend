using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Storage;
using StackExchange.Redis;

namespace Shared.AzureBlob;

public static class Extensions
{
    public static IServiceCollection AddAzureBlob(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string? connectionString = configuration.GetConnectionString("azureblob");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Azure connection string is not configured.");
        }

        services.AddSingleton<IStorage>(new AzureBlobService(connectionString));

        return services;
    }
}
