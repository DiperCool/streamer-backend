using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Models;
using Streamers.Features.ModerationActivities.Models;
using Streamers.Features.Persistence;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.ModerationActivities.Features.BanUserAction;

public class UserBannedEventHandler(StreamerDbContext dbContext) : IDomainEventHandler<UserBanned>
{
    public async Task Handle(UserBanned notification, CancellationToken cancellationToken) // Changed signature
    {
        var banAction = new BanAction(
            moderatorId: notification.BannedUser.BannedById,
            streamerId: notification.BannedUser.BroadcasterId,
            targetUserId: notification.BannedUser.UserId,
            bannedUntil: notification.BannedUser.BannedUntil,
            reason: notification.BannedUser.Reason
        );

        await dbContext.ModeratorActionTypes.AddAsync(banAction, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
