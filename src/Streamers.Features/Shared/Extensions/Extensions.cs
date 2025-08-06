using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Abstractions.Cqrs;
using Shared.DomainEvents;
using Shared.Minio;
using Shared.RabbitMQ.Extensions;
using Shared.Seeds;
using streamer.ServiceDefaults;
using Streamers.Features.Files.Seeds;
using Streamers.Features.Shared.Data;
using Streamers.Features.Shared.GraphQl;
using Streamers.Features.Shared.Persistence;
using Streamers.Features.Streamers.EventHandlers;

namespace Streamers.Features.Shared.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddDbContext<StreamerDbContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("streamerdb");
            options.UseNpgsql(connectionString);
        });
        services.AddScoped<IUserEventHandler, UserEventHandler>();
        builder.Services.AddHostedService<MigrationWorker<StreamerDbContext>>();
        builder.Services.AddHostedService<SeedWorker>();
        builder.Services.AddScoped<IDataSeeder, MinioBucketSeeds>();
        builder.Services.AddMediator(typeof(Features).Assembly);
        builder.Services.AddDomainEvents();
        builder.Services.AddBlobStorage(builder.Configuration);

        builder.Services.AddGraphQl();
        // Add the authentication services to DI
        builder.AddDefaultAuthentication();
        builder.Services.AddRabbitMq(builder.Configuration);
    }
}
