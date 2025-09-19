using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Notifications.Features.EditNotificationSettings;
using Streamers.Features.Notifications.Features.ReadAllNotifications;
using Streamers.Features.Notifications.Features.ReadNotification;

namespace Streamers.Features.Notifications.Graphql;

[MutationType]
[Authorize]
public static partial class NotificationMutations
{
    public static async Task<ReadNotificationResponse> ReadNotification(
        ReadNotification readNotification,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(readNotification);
    }

    public static async Task<ReadAllNotificationsResponse> ReadAllNotifications(
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(new ReadAllNotifications());
    }

    public static async Task<EditNotificationSettingsResponse> EditNotificationSettings(
        EditNotificationSettings readNotification,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(readNotification);
    }
}
