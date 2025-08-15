using HotChocolate;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;
using Streamers.Features.Streams.Dtos;

namespace Streamers.Features.Streams.GraphQl;

[ObjectType<StreamDto>]
public static partial class StreamType
{
    public static async Task<StreamerDto?> GetStreamer(
        [Parent(nameof(StreamDto.StreamerId))] StreamDto stream,
        IStreamersByIdDataLoader streamerDataLoader,
        CancellationToken cancellationToken
    )
    {
        var streamer = await streamerDataLoader.LoadAsync(stream.StreamerId, cancellationToken);
        return (StreamerDto?)streamer;
    }
}
