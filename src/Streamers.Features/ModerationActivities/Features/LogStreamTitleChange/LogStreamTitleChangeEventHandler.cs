using Shared.Abstractions.Domain;
using Streamers.Features.ModerationActivities.Models;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.StreamInfos.Models;

namespace Streamers.Features.ModerationActivities.Features.LogStreamTitleChange;

public class LogStreamTitleChangeEventHandler(StreamerDbContext dbContext)
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
    }
}
