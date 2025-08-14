using HotChocolate.Subscriptions;
using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Streamers.EventHandlers;

public class StreamerUpdatedEventHandler(ITopicEventSender sender)
    : IDomainEventHandler<StreamerUpdated>
{
    public async Task Handle(StreamerUpdated domainEvent, CancellationToken cancellationToken)
    {
        var streamer = domainEvent.Streamer;
        var streamerDto = new StreamerDto
        {
            Id = streamer.Id,
            UserName = streamer.UserName,
            Avatar = streamer.Avatar,
            Followers = streamer.Followers,
        };
        await sender.SendAsync(streamerDto.Id, streamer, cancellationToken);
    }
}
