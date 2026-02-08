using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Streams.Dtos;
using Streamers.Features.Streams.Features.GetCurrentStream;
using Streamers.Features.Streams.Features.GetStreams;
using Streamers.Features.Streams.Features.GetStreamSettings;
using Streamers.Features.Streams.Features.GetTopStreams;

namespace Streamers.Features.Streams.GraphQl;

[QueryType]
public static partial class StreamQuery
{
    [Authorize]
    public static async Task<StreamSettingsDto> GetStreamSettings(IMediator mediator)
    {
        return await mediator.Send(new GetStreamSettings());
    }

    public static async Task<StreamDto> GetCurrentStream(string streamerId, IMediator mediator)
    {
        return await mediator.Send(new GetCurrentStream(streamerId));
    }

    public static async Task<List<StreamDto>> GetTopStreams(IMediator mediator)
    {
        return await mediator.Send(new GetTopStreams());
    }

    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<StreamDto>> GetStreams(
        Guid? categoryId,
        Guid? tag,
        List<string>? languages,
        [Service] IMediator mediator,
        QueryContext<StreamDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetStreams(categoryId, tag, languages ?? [], offsetPagingArguments, rcontext),
            cancellationToken
        );
        return result.ToConnection();
    }
}
