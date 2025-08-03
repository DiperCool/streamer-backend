using MassTransit;
using streamer.EventBus.Abstractions;
using streamer.EventBus.Events;

namespace streamer.EventBus;

public class MassTransitBus : IEventBus
{
    IPublishEndpoint _endpoint;

    public MassTransitBus(IPublishEndpoint endpoint)
    {
        _endpoint = endpoint;
    }

    public async Task PublishAsync(IntegrationEvent @event)
    {
        await _endpoint.Publish(@event);
    }
}
