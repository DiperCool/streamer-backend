using System.Runtime.CompilerServices;
using System.Security.Claims;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using streamer.ServiceDefaults;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.ModerationActivities.Dtos;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared;

namespace Streamers.Features.ModerationActivities.Graphql;

[SubscriptionType]
public static partial class ModerationActivitySubscription
{
    [GraphQLIgnore]
    public static async IAsyncEnumerable<ModeratorActionDto> SubscribeModerationActivityCreated(
        [Service] ITopicEventReceiver receiver,
        [GlobalState(nameof(ClaimsPrincipal))] ClaimsPrincipal user,
        [Service] IRoleService roleService,
        string streamerId,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            yield break;
        }

        if (!await roleService.HasRole(streamerId, user.GetUserId(), Permissions.Chat))
        {
            yield break;
        }

        string topic = $"{Constants.ModerationActivityCreated}-{streamerId}";

        ISourceStream<ModeratorActionDto> stream =
            await receiver.SubscribeAsync<ModeratorActionDto>(topic, cancellationToken);

        await foreach (
            ModeratorActionDto evt in stream.ReadEventsAsync().WithCancellation(cancellationToken)
        )
        {
            yield return evt;
        }
    }

    [Subscribe(With = nameof(SubscribeModerationActivityCreated))]
    public static ModeratorActionDto ModerationActivityCreated(
        string streamerId,
        [EventMessage] ModeratorActionDto moderationActivity
    ) => moderationActivity;
}
