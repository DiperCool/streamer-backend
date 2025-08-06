using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using streamer.ServiceDefaults;

namespace Shared.Minio;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBlobStorage(
        this IServiceCollection builder,
        IConfiguration configuration
    )
    {
        builder.AddSingleton<IMinioClient>(serviceProvider =>
        {
            var minio = configuration.BindOptions<MinioOptions>();
            var minioClient = new MinioClient()
                .WithEndpoint(minio.Uri ?? "")
                .WithCredentials(minio.Username, minio.Password)
                .Build();
            minioClient.WithTimeout(5000);
            return minioClient;
        });
        builder.AddTransient<IMinioService, MinioService>();
        return builder;
    }
}
