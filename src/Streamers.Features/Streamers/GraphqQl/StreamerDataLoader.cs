using GreenDonut;
using HotChocolate;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.Features.GetStreamersByIds;

namespace Streamers.Features.Streamers.GraphqQl;

public static partial class StreamerDataLoader
{
    [DataLoader]
    public static async Task<IDictionary<string, StreamerDto>> GetStreamersByIdAsync(
        [Service] IMediator mediator,
        IReadOnlyList<string> ids,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new GetStreamersByIds(ids), cancellationToken);
        return response.Streamers;
    }
}
