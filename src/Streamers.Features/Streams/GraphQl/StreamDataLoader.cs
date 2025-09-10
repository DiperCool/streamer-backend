using GreenDonut;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Streams.Dtos;
using Streamers.Features.Streams.Features.GetStreamsByIds;

namespace Streamers.Features.Streams.GraphQl;

public static partial class StreamDataLoader
{
    [DataLoader]
    public static async Task<IDictionary<Guid, StreamDto>> GetStreamById(
        IReadOnlyList<Guid> streamsIds,
        IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(new GetStreamsByIds(streamsIds), cancellationToken);
        return result.Streams;
    }
}
