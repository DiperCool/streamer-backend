using HotChocolate;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.StreamInfos.Features.UpdateStreamInfo;

namespace Streamers.Features.StreamInfos.Graphql;

[MutationType]
public static partial class StreamInfoMutations
{
    public static async Task<UpdateStreamInfoResponse> UpdateStreamInfo(
        UpdateStreamInfo streamInfo,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(streamInfo);
    }
}
