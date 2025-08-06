using Shared.RabbitMQ.Events;

namespace Shared.RabbitMQ.Abstractions;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent @event);
}
