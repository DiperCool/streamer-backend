using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Storage;
using streamer.ServiceDefaults;

namespace Shared.S3;

public static class Extensions
{
    public static IServiceCollection AddS3(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var conf = sp.GetRequiredService<IConfiguration>();
            var opt = conf.BindOptions<S3Options>();
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(opt.Region),
            };

            return new AmazonS3Client(opt.AccessKeyId, opt.SecretAccessKey, config);
        });

        services.AddScoped<IStorage, S3Service>();
        return services;
    }
}
