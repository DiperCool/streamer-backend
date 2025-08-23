using HotChocolate.Subscriptions;
using Shared.Abstractions.Domain;
using Streamers.Features.Streams.Dtos;
using Streamers.Features.Streams.Models;

namespace Streamers.Features.Streams.EventHandlers;

public class StreamerUpdatedEventHandler(ITopicEventSender sender)
    : IDomainEventHandler<StreamUpdated>
{
    public async Task Handle(StreamUpdated domainEvent, CancellationToken cancellationToken)
    {
        var stream = domainEvent.Stream;
        var streamDto = new StreamDto
        {
            Id = stream.Id,

            StreamerId = stream.StreamerId,
            Active = stream.Active,
            Title = stream.Title,
            CurrentViewers = stream.CurrentViewers,
        };
        await sender.SendAsync(
            $"{nameof(StreamUpdated)}-{streamDto.Id}",
            streamDto,
            cancellationToken
        );
    }
}
