using HotChocolate.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Domain;
using Streamers.Features.Notifications.Dtos;
using Streamers.Features.Notifications.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Notifications.Features.CreateNotification.Events;

public class NotificationCreatedEventHandler(
    ITopicEventSender sender,
    StreamerDbContext streamerDbContext
) : IDomainEventHandler<NotificationCreated>
{
    public async Task Handle(NotificationCreated domainEvent, CancellationToken cancellationToken)
    {
        var notification = domainEvent.Notification;
        NotificationDto notificationSend = new NotificationDto
        {
            Id = notification.Id,
            CreatedAt = notification.CreatedAt,
            Seen = notification.Seen,
            Discriminator = notification.Discriminator,
            StreamerId =
                notification.Discriminator == nameof(LiveStartedNotification)
                    ? ((LiveStartedNotification)notification).StreamerId
                    : ((UserFollowedNotification)notification).StreamerId,
        };
        await streamerDbContext.Streamers.ExecuteUpdateAsync(
            x => x.SetProperty(a => a.HasUnreadNotifications, true),
            cancellationToken: cancellationToken
        );
        await streamerDbContext.SaveChangesAsync(cancellationToken);

        await sender.SendAsync(
            $"{nameof(NotificationCreated)}-{notification.UserId}",
            notificationSend,
            cancellationToken
        );
    }
}
