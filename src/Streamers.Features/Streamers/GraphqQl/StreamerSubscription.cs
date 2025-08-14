using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Streamers.Dtos;

namespace Streamers.Features.Streamers.GraphqQl;

[SubscriptionType]
public static partial class StreamerSubscription
{
    [Subscribe]
    [Topic($"{{{nameof(streamerId)}}}")]
    public static StreamerDto StreamerUpdated(
        string streamerId,
        [EventMessage] StreamerDto streamer
    ) => streamer;
}
