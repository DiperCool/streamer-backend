using GreenDonut;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Streams.Dtos;
using Streamers.Features.Streams.Features.GetSourcesByStream;

namespace Streamers.Features.Streams.GraphQl;

public static partial class StreamSourcesDataLoader
{
    [DataLoader]
    public static async Task<ILookup<Guid, StreamSourceDto>> GetSourcesByStreamIds(
        IReadOnlyList<Guid> streamsIds,
        IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(new GetSourcesByStream(streamsIds), cancellationToken);
        return result;
    }
}
