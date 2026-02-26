using HotChocolate.Subscriptions;
using Streamers.Features.ModerationActivities.Dtos;
using Streamers.Features.Shared;

namespace Streamers.Features.ModerationActivities.Services;

public class ModerationActivityEventPublisher(ITopicEventSender sender)
    : IModerationActivityEventPublisher
{
    public async Task PublishModerationActivityCreatedAsync(
        ModeratorActionDto dto,
        CancellationToken cancellationToken = default
    )
    {
        await sender.SendAsync(
            $"{nameof(Constants.ModerationActivityCreated)}-{dto.StreamerId}",
            dto,
            cancellationToken
        );
    }
}
