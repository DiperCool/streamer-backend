using System.ComponentModel.DataAnnotations;
using System.Reflection;
using MassTransit;
using MassTransit.Metadata;
using Microsoft.Extensions.Hosting;
using streamer.EventBus.Events;
using streamer.EventBus.Options;
using streamer.ServiceDefaults;

namespace streamer.EventBus.Extensions;

public static class Extensions
{
    public static IHostApplicationBuilder AddMassTransit(this IHostApplicationBuilder builder,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? configureReceiveEndpoints = null,
        Action<IBusRegistrationConfigurator>? configureBusRegistration = null,
        Assembly? scanAssemblies = null
    )
    {
     
        
        var options = builder.Configuration.BindOptions<EventBusOptions>();
        builder.Services.AddMassTransit(x =>
        {
            scanAssemblies ??= Assembly.GetEntryAssembly();
            if (scanAssemblies == null)
            {
                throw new InvalidOperationException("Could not find assembly entry assembly.");
            }
            IEnumerable<Type> types = scanAssemblies.GetTypes().Where(RegistrationMetadata.IsConsumerOrDefinition);
            x.AddConsumers(types.ToArray());
            configureBusRegistration?.Invoke(x);
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.UseConcurrencyLimit(10);
                cfg.ConfigureEndpoints(context);
                cfg.PublishTopology.BrokerTopologyOptions = PublishBrokerTopologyOptions.FlattenHierarchy;
                cfg.Host(options.Host, options.VirtualHost, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });
                cfg.Publish<IntegrationEvent>(p => p.Exclude = true);
                cfg.UseMessageRetry(r => AddRetryConfiguration(r));
                configureReceiveEndpoints?.Invoke(context, cfg);
            });
        });
        return builder;
    }
    private static IRetryConfigurator AddRetryConfiguration(IRetryConfigurator retryConfigurator)
    {
        retryConfigurator
            .Exponential(3, TimeSpan.FromMilliseconds(200), TimeSpan.FromMinutes(120), TimeSpan.FromMilliseconds(200))
            .Ignore<ValidationException>(); // don't retry if we have invalid data and message goes to _error queue masstransit

        return retryConfigurator;
    }
}
