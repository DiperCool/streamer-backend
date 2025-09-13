using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.Features.GetBannedUsers;

namespace Streamers.Features.Chats.GraphQl;

[QueryType]
public static partial class BannedUserQuery
{
    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<BannedUserDto>> GetBannedUsers(
        string streamerId,
        [Service] IMediator mediator,
        QueryContext<BannedUserDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetBannedUsers(streamerId, offsetPagingArguments, rcontext),
            cancellationToken
        );
        return result.ToConnection();
    }
}
