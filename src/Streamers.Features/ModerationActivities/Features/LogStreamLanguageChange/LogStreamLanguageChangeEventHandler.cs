using Shared.Abstractions.Domain;
using Streamers.Features.ModerationActivities.Models;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.StreamInfos.Models;

namespace Streamers.Features.ModerationActivities.Features.LogStreamLanguageChange;

public class LogStreamLanguageChangeEventHandler(StreamerDbContext dbContext)
    : IDomainEventHandler<StreamLanguageChanged>
{
    public async Task Handle(
        StreamLanguageChanged notification,
        CancellationToken cancellationToken
    )
    {
        var action = new StreamLanguageAction(
            moderatorId: notification.ModeratorId,
            streamerId: notification.StreamerId,
            newLanguage: notification.NewLanguage
        );

        await dbContext.ModeratorActionTypes.AddAsync(action, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
