using FluentValidation;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Abstractions.Cqrs;
using Shared.DomainEvents;
using Shared.Hangfire;
using Shared.Minio;
using Shared.RabbitMQ.Extensions;
using Shared.Redis;
using Shared.Seeds;
using streamer.ServiceDefaults;
using Streamers.Features.AntMedia.EventHandlers;
using Streamers.Features.AntMedia.Services;
using Streamers.Features.Files.Seeds;
using Streamers.Features.Profiles.Features.UpdateProfile;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Cqrs.Behaviours;
using Streamers.Features.Shared.GraphQl;
using Streamers.Features.Shared.Hangfire;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.EventHandlers;
using Streamers.Features.Streamers.Services;
using Streamers.Features.Streams.Features;
using Streamers.Features.Vods.EventHandler;

namespace Streamers.Features;

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
        services.AddScoped<IAntmediaWebhook, AntmediaWebhook>();
        services.AddScoped<IVodFinishedHandler, VodFinishedHandler>();
        services.AddSingleton<IStreamKeyGenerator, StreamKeyGenerator>();
        services.AddScoped<IAntMediaWebhookHandlerFabric, AntMediaWebhookHandlerFabric>();
        services.AddScoped<LiveStreamStartedHandler>();
        services.AddScoped<LiveStreamEndedHandler>();
        services.AddScoped<AddReaderHandler>();
        services.AddScoped<RemoveReaderHandler>();
        services.AddScoped<ICanAssignRole, CanAssignRole>();
        services.AddScoped<IRolesHierarchy, RolesHierarchy>();

        services.AddHostedService<RecurringJobsHostedService>();
        builder.Services.AddHostedService<MigrationWorker<StreamerDbContext>>();
        builder.Services.AddHostedService<SeedWorker>();
        builder.Services.AddScoped<IDataSeeder, MinioBucketSeeds>();
        builder.Services.AddMediator(typeof(Features).Assembly);
        builder.Services.AddDomainEvents(typeof(Features).Assembly);
        builder.Services.AddBlobStorage(builder.Configuration);
        services.AddValidatorsFromAssemblyContaining<UpdateProfile.UpdateProfileValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        builder.Services.AddGraphQl();
        // Add the authentication services to DI
        builder.AddDefaultAuthentication();
        builder.Services.AddRabbitMq(builder.Configuration);
        builder.Services.AddRedis(builder.Configuration);
        builder.Services.AddHangfire(builder.Configuration);
    }
}
