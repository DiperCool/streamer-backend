using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Streams.Dtos;

namespace Streamers.Features.Streams.GraphQl;

[SubscriptionType]
public static partial class StreamSubscription
{
    [Subscribe]
    [Topic($"{nameof(StreamUpdated)}-{{{nameof(streamId)}}}")]
    public static StreamDto StreamUpdated(Guid streamId, [EventMessage] StreamDto streamer) =>
        streamer;
}
