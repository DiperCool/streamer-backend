using Hangfire;
using Shared.Abstractions.Domain;
using Streamers.Features.Notifications.Job;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Notifications.Features.CreateLiveStartedNotification.Events;

public class StreamerUpdatedEventHandler() : IDomainEventHandler<StreamerUpdated>
{
    public Task Handle(StreamerUpdated domainEvent, CancellationToken cancellationToken)
    {
        if (!domainEvent.Streamer.IsLive)
        {
            return Task.CompletedTask;
        }
        BackgroundJob.Enqueue<NotifyFollowersStreamerLiveJob>(x =>
            x.NotifyFollowersStreamerLive(domainEvent.Streamer.Id)
        );
        return Task.CompletedTask;
    }
}
