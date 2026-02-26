using Shared.Abstractions.Domain;
using Streamers.Features.ModerationActivities.Models;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.StreamInfos.Models;

using Streamers.Features.ModerationActivities.Dtos;
using Streamers.Features.ModerationActivities.Services;

namespace Streamers.Features.ModerationActivities.Features.LogStreamTitleChange;

public class LogStreamTitleChangeEventHandler(StreamerDbContext dbContext, IModerationActivityEventPublisher publisher)
    : IDomainEventHandler<StreamTitleChanged>
{
    public async Task Handle(StreamTitleChanged notification, CancellationToken cancellationToken)
    {
        var action = new StreamNameAction(
            moderatorId: notification.ModeratorId,
            streamerId: notification.StreamerId,
            newStreamName: notification.NewTitle
        );

        await dbContext.ModeratorActionTypes.AddAsync(action, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var actionDto = new StreamNameActionDto
        {
            Id = action.Id,
            Name = action.Name,
            ModeratorId = action.ModeratorId,
            StreamerId = action.StreamerId,
            CreatedDate = action.CreatedDate,
            NewStreamName = action.NewStreamName,
        };

        await publisher.PublishModerationActivityCreatedAsync(actionDto, cancellationToken);
    }
}
