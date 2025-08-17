using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Streams.Dtos;

namespace Streamers.Features.Streams.GraphQl;

[SubscriptionType]
public static partial class StreamSubscription
{
    [Subscribe]
    [Topic($"{{{nameof(streamId)}}}")]
    public static StreamDto StreamUpdated(Guid streamId, [EventMessage] StreamDto streamer) =>
        streamer;
}
