using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.StreamInfos.Dtos;
using Streamers.Features.StreamInfos.Features.GetStreamInfo;

namespace Streamers.Features.StreamInfos.Graphql;

[QueryType]
public static partial class StreamInfoQuery
{
    public static async Task<StreamInfoDto> GetStreamInfoAsync(
        string streamerId,
        IMediator mediator
    )
    {
        return await mediator.Send(new GetStreamInfo(streamerId));
    }
}
