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
        IStreamersByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        var streamer = await dataLoader.LoadAsync(stream.StreamerId, cancellationToken);
        return streamer;
    }

    public static async Task<StreamSourceDto[]> GetSources(
        [Parent] StreamDto stream,
        ISourcesByStreamIdsDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        var streamers = await dataLoader.LoadAsync(stream.Id, cancellationToken);
        return streamers ?? [];
    }
}
