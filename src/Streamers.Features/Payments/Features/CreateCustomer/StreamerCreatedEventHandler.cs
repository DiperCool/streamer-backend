using DotNetCore.CAP;
using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Payments.Features.CreateCustomer;

public class StreamerCreatedEventHandler(ICapPublisher capPublisher)
    : IDomainEventHandler<StreamerCreated>
{
    public async Task Handle(StreamerCreated domainEvent, CancellationToken cancellationToken)
    {
        await capPublisher.PublishAsync(
            "streamer.created",
            new StreamerEvent { Id = domainEvent.Streamer.Id, Email = domainEvent.Streamer.Email },
            cancellationToken: cancellationToken
        );
    }
}
