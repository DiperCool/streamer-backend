using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Vods.Dtos;
using Streamers.Features.Vods.Features.GetVod;
using Streamers.Features.Vods.Features.GetVods;
using Streamers.Features.Vods.Features.GetVodSettings;

namespace Streamers.Features.Vods.GraphQl;

[QueryType]
public static partial class VodQuery
{
    public static async Task<VodDto> GetVodAsync(Guid vodId, [Service] IMediator mediator)
    {
        return await mediator.Send(new GetVod(vodId));
    }

    [Authorize]
    public static async Task<VodSettingsDto> GetVodSettingsAsync([Service] IMediator mediator)
    {
        return await mediator.Send(new GetVodSettings());
    }

    [UsePaging(MaxPageSize = 10)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<VodDto>> GetVodsAsync(
        string streamerId,
        [Service] IMediator mediator,
        QueryContext<VodDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetVods(streamerId, offsetPagingArguments, rcontext),
            cancellationToken
        );
        return result.ToConnection();
    }
}
