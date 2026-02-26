using Streamers.Features.ModerationActivities.Dtos;

namespace Streamers.Features.ModerationActivities.Services;

public interface IModerationActivityEventPublisher
{
    Task PublishModerationActivityCreatedAsync(ModeratorActionDto dto, CancellationToken cancellationToken = default);
}
