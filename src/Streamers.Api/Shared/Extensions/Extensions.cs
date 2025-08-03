using MassTransit;
using Microsoft.EntityFrameworkCore;
using streamer.EventBus.Extensions;
using streamer.ServiceDefaults;
using Streamers.Api.Shared.Data;
using Streamers.Api.Streamers.EventHandlers;
using Streamers.Api.Streamers.Events;

namespace Streamers.Api.Shared.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddDbContext<StreamerDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("streamerdb"));
        });
        builder.Services.AddHostedService<MigrationWorker<StreamerDbContext>>();

        // Add the authentication services to DI
        builder.AddDefaultAuthentication();
        builder.AddMassTransit(configureReceiveEndpoints: ((context, cfg) =>
        {
            cfg.ReceiveEndpoint(
                nameof(UserCreated),
                re =>
                {
                    re.SetQuorumQueue();
                    re.ConfigureConsumeTopology = false;
                    re.ConfigureConsumer<CreateUser>(context);
                    
                    re.ClearSerialization();
                    re.UseRawJsonSerializer();
                    re.RethrowFaultedMessages();
                    if (re is { } rabbit) 
                    {
                        rabbit.Bind("amq.topic", ex =>
                        {
                            ex.ExchangeType = "topic";
                            ex.RoutingKey = "KK.EVENT.CLIENT.Streamers.SUCCESS.Streamers.REGISTER";
                        });
                    }
                }
            );
        }));
        services.AddHttpContextAccessor();
    }
}
