using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Notifications.Features.ReadNotification;

public record ReadNotificationResponse(bool HasUnreadNotifications);

public record ReadNotification(Guid Id) : IRequest<ReadNotificationResponse>;

public class ReadNotificationsHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<ReadNotification, ReadNotificationResponse>
{
    public async Task<ReadNotificationResponse> Handle(
        ReadNotification request,
        CancellationToken cancellationToken
    )
    {
        var notification = await streamerDbContext
            .Notifications.Include(x => x.User)
            .Where(n => n.UserId == currentUser.UserId && n.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (notification == null)
        {
            throw new InvalidOperationException($"Notification with id {request.Id} not found");
        }

        notification.Seen = true;
        streamerDbContext.Notifications.Update(notification);
        await streamerDbContext.SaveChangesAsync(cancellationToken);

        var anyUnseen = await streamerDbContext.Notifications.AnyAsync(
            x => x.UserId == currentUser.UserId && !x.Seen,
            cancellationToken: cancellationToken
        );
        await streamerDbContext.Streamers.ExecuteUpdateAsync(
            x => x.SetProperty(a => a.HasUnreadNotifications, anyUnseen),
            cancellationToken: cancellationToken
        );
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new ReadNotificationResponse(anyUnseen);
    }
}
