using Shared.Abstractions.Domain;
using Streamers.Features.ModerationActivities.Models;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.StreamInfos.Models;

namespace Streamers.Features.ModerationActivities.Features.LogStreamCategoryChange;

public class LogStreamCategoryChangeEventHandler(StreamerDbContext dbContext)
    : IDomainEventHandler<StreamCategoryChanged>
{
    public async Task Handle(
        StreamCategoryChanged notification,
        CancellationToken cancellationToken
    )
    {
        if (notification.NewCategoryId is null)
        {
            return;
        }

        var action = new StreamCategoryAction(
            moderatorId: notification.ModeratorId,
            streamerId: notification.StreamerId,
            newCategory: notification.NewCategoryId.Value.ToString()
        );

        await dbContext.ModeratorActionTypes.AddAsync(action, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
