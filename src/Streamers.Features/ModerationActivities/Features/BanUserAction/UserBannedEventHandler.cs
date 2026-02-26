using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Models;
using Streamers.Features.ModerationActivities.Models;
using Streamers.Features.Persistence;
using Streamers.Features.Shared.Persistance;

using Streamers.Features.ModerationActivities.Dtos;
using Streamers.Features.ModerationActivities.Services;

namespace Streamers.Features.ModerationActivities.Features.BanUserAction;

public class UserBannedEventHandler(StreamerDbContext dbContext, IModerationActivityEventPublisher publisher) : IDomainEventHandler<UserBanned>
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

        var banActionDto = new BanActionDto
        {
            Id = banAction.Id,
            Name = banAction.Name,
            ModeratorId = banAction.ModeratorId,
            StreamerId = banAction.StreamerId,
            CreatedDate = banAction.CreatedDate,
            TargetUserId = banAction.TargetUserId,
            BannedUntil = banAction.BannedUntil,
            Reason = banAction.Reason,
        };

        await publisher.PublishModerationActivityCreatedAsync(banActionDto, cancellationToken);
    }
}
