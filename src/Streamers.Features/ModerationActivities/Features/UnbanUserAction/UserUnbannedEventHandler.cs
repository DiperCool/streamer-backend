using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Models;
using Streamers.Features.ModerationActivities.Dtos;
using Streamers.Features.ModerationActivities.Models;
using Streamers.Features.ModerationActivities.Services;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.ModerationActivities.Features.UnbanUserAction;

public class UserUnbannedEventHandler(
    StreamerDbContext dbContext,
    IModerationActivityEventPublisher publisher
) : IDomainEventHandler<UserUnbanned>
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

        var unbanActionDto = new UnbanActionDto
        {
            Id = unbanAction.Id,
            Name = unbanAction.Name,
            ModeratorId = unbanAction.ModeratorId,
            StreamerId = unbanAction.StreamerId,
            CreatedDate = unbanAction.CreatedDate,
            TargetUserId = unbanAction.TargetUserId,
        };

        await publisher.PublishModerationActivityCreatedAsync(unbanActionDto, cancellationToken);
    }
}
