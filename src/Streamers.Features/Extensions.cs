using FluentValidation;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.Abstractions.Cqrs;
using Shared.Auth0.Services;
using Shared.AzureBlob;
using Shared.DomainEvents;
using Shared.Hangfire;
using Shared.RabbitMQ.Extensions;
using Shared.Redis;
using Shared.S3;
using Shared.Seeds;
using streamer.ServiceDefaults;
using Streamers.Features.AntMedia.EventHandlers;
using Streamers.Features.AntMedia.Services;
using Streamers.Features.Categories.Services;
using Streamers.Features.Chats.Services;
using Streamers.Features.Files.Seeds;
using Streamers.Features.Notifications.Services;
using Streamers.Features.Profiles.Features.UpdateProfile;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Cqrs.Behaviours;
using Streamers.Features.Shared.GraphQl;
using Streamers.Features.Shared.Hangfire;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.EventHandlers;
using Streamers.Features.Streamers.Seed;
using Streamers.Features.Streamers.Services;
using Streamers.Features.Streams.Features;
using Streamers.Features.SystemRoles.Services;
using Streamers.Features.Tags.Services;
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
        services.AddScoped<IStreamerFabric, StreamerFabric>();
        services.AddScoped<IAntMediaWebhookHandlerFabric, AntMediaWebhookHandlerFabric>();
        services.AddScoped<LiveStreamStartedHandler>();
        services.AddScoped<LiveStreamEndedHandler>();
        services.AddScoped<AddReaderHandler>();
        services.AddScoped<RemoveReaderHandler>();
        services.AddScoped<ICanAssignRole, CanAssignRole>();
        services.AddScoped<IRolesHierarchy, RolesHierarchy>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IChatPermissionService, ChatPermissionService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IChatRule, BanChatRule>();
        services.AddScoped<IChatRule, OnlyFollowerModeChatRule>();
        services.AddScoped<IChatRule, SlowModeChatRule>();

        services.AddHostedService<RecurringJobsHostedService>();
        builder.Services.AddHostedService<MigrationWorker<StreamerDbContext>>();
        builder.Services.AddHostedService<SeedWorker>();
        builder.Services.AddScoped<IDataSeeder, MinioBucketSeeds>();
        builder.Services.AddScoped<ISystemRoleService, SystemRoleService>();
        builder.Services.AddSingleton<ISlugGenerator, SlugGenerator>();
        builder.Services.AddScoped<ITagsService, TagsService>();
        builder.Services.AddMediator(typeof(Features).Assembly);
        builder.Services.AddDomainEvents(typeof(Features).Assembly);
        builder.Services.AddS3(builder.Configuration);
        services.AddValidatorsFromAssemblyContaining<UpdateProfile.UpdateProfileValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        builder.Services.AddGraphQl();
        // Add the authentication services to DI
        builder.AddDefaultAuthentication();
        builder.Services.AddRabbitMq(builder.Configuration);
        builder.Services.AddRedis(builder.Configuration);
        builder.Services.AddAuth0Apis(builder.Configuration);
        builder.Services.AddHangfire(builder.Configuration);
    }
}
