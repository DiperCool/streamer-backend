using streamer.EventBus.Events;

namespace streamer.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent @event);
}
