using Shared.Abstractions.Domain;
using Streamers.Features.ModerationActivities.Models;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.StreamInfos.Models;

using Streamers.Features.ModerationActivities.Dtos;
using Streamers.Features.ModerationActivities.Services;

namespace Streamers.Features.ModerationActivities.Features.LogStreamLanguageChange;

public class LogStreamLanguageChangeEventHandler(StreamerDbContext dbContext, IModerationActivityEventPublisher publisher)
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

        var actionDto = new StreamLanguageActionDto
        {
            Id = action.Id,
            Name = action.Name,
            ModeratorId = action.ModeratorId,
            StreamerId = action.StreamerId,
            CreatedDate = action.CreatedDate,
            NewLanguage = action.NewLanguage,
        };

        await publisher.PublishModerationActivityCreatedAsync(actionDto, cancellationToken);
    }
}
