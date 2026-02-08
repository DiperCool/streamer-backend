using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Followers.Dtos;
using Streamers.Features.Followers.Features.GetMyFollowers;
using Streamers.Features.Followers.Features.GetMyFollowings;

namespace Streamers.Features.Followers.GraphQl;

[QueryType]
public static partial class StreamerQuery
{
    [Authorize]
    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<StreamerFollowerDto>> GetMyFollowings(
        string? search,
        [Service] IMediator mediator,
        QueryContext<StreamerFollowerDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetMyFollowings(search, rcontext, offsetPagingArguments),
            cancellationToken
        );
        return result.ToConnection();
    }

    [Authorize]
    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<FollowerDto>> GetMyFollowers(
        string? search,
        [Service] IMediator mediator,
        QueryContext<FollowerDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetMyFollowers(search, rcontext, offsetPagingArguments),
            cancellationToken
        );
        return result.ToConnection();
    }
}
