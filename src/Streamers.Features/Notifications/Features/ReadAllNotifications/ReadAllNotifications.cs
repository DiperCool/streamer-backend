using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Notifications.Features.ReadAllNotifications;

public record ReadAllNotificationsResponse(bool Result);

public record ReadAllNotifications() : IRequest<ReadAllNotificationsResponse>;

public class ReadAllNotificationsHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser
) : IRequestHandler<ReadAllNotifications, ReadAllNotificationsResponse>
{
    public async Task<ReadAllNotificationsResponse> Handle(
        ReadAllNotifications request,
        CancellationToken cancellationToken
    )
    {
        await streamerDbContext
            .Notifications.Where(x => !x.Seen && x.UserId == currentUser.UserId)
            .ExecuteUpdateAsync(
                x => x.SetProperty(a => a.Seen, true),
                cancellationToken: cancellationToken
            );
        await streamerDbContext
            .Streamers.Where(x => x.Id == currentUser.UserId)
            .ExecuteUpdateAsync(
                x => x.SetProperty(a => a.HasUnreadNotifications, false),
                cancellationToken: cancellationToken
            );
        return new ReadAllNotificationsResponse(true);
    }
}
