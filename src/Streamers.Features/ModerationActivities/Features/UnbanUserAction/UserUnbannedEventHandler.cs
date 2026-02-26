using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Models; // For UserUnbanned event
using Streamers.Features.ModerationActivities.Models;
using Streamers.Features.Shared.Persistance; // Assuming DbContext is here

namespace Streamers.Features.ModerationActivities.Features.UnbanUserAction;

public class UserUnbannedEventHandler(StreamerDbContext dbContext)
    : IDomainEventHandler<UserUnbanned>
{
    public async Task Handle(UserUnbanned notification, CancellationToken cancellationToken)
    {
        var unbanAction = new UnbanAction(
            moderatorId: notification.BannedUser.BannedById,
            streamerId: notification.BannedUser.BroadcasterId,
            targetUserId: notification.BannedUser.UserId
        );

        await dbContext.ModeratorActionTypes.AddAsync(unbanAction, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
