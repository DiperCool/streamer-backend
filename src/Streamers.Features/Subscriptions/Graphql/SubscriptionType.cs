using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;
using Streamers.Features.Subscriptions.Dtos;

namespace Streamers.Features.Subscriptions.Graphql;

[ObjectType<SubscriptionDto>]
public static partial class SubscriptionType
{
    public static async Task<StreamerDto?> GetStreamerAsync(
        [Parent] SubscriptionDto subscription,
        IStreamersByIdDataLoader dataLoader
    )
    {
        return await dataLoader.LoadAsync(subscription.StreamerId);
    }

    public static async Task<StreamerDto?> GetUserAsync(
        [Parent] SubscriptionDto subscription,
        IStreamersByIdDataLoader dataLoader
    )
    {
        return await dataLoader.LoadAsync(subscription.UserId);
    }
}
