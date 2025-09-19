using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Notifications.Dtos;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;

namespace Streamers.Features.Notifications.Graphql;

[ObjectType<NotificationDto>]
public static partial class NotificationType
{
    public static async Task<StreamerDto?> GetStreamerAsync(
        [Parent] NotificationDto notification,
        [Service] IStreamersByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        if (notification.StreamerId == null)
        {
            return null;
        }
        return await dataLoader.LoadAsync(notification.StreamerId, cancellationToken);
    }
}
