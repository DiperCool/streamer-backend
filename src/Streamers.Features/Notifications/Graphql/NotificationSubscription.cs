using System.Runtime.CompilerServices;
using System.Security.Claims;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using streamer.ServiceDefaults;
using Streamers.Features.Notifications.Dtos;

namespace Streamers.Features.Notifications.Graphql;

[SubscriptionType]
public static partial class NotificationSubscription
{
    public static async IAsyncEnumerable<NotificationDto> SubscribeNotificationCreated(
        [Service] ITopicEventReceiver receiver,
        [GlobalState(nameof(ClaimsPrincipal))] ClaimsPrincipal user,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            yield break;
        }
        string topic = $"{nameof(NotificationCreated)}-{user.GetUserId()}";

        ISourceStream<NotificationDto> stream = await receiver.SubscribeAsync<NotificationDto>(
            topic,
            cancellationToken
        );

        await foreach (
            NotificationDto evt in stream.ReadEventsAsync().WithCancellation(cancellationToken)
        )
        {
            yield return evt;
        }
    }

    [Subscribe(With = nameof(SubscribeNotificationCreated))]
    public static NotificationDto NotificationCreated(
        [EventMessage] NotificationDto notification
    ) => notification;
}
