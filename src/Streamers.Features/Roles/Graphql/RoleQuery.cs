using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Dtos;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Features;
using Streamers.Features.Roles.Features.GetMyRole;
using Streamers.Features.Roles.Features.GetMyRoles;
using Streamers.Features.Roles.Features.GetRoles;
using RoleType = Streamers.Features.Roles.Enums.RoleType;

namespace Streamers.Features.Roles.Graphql;

[QueryType]
public static partial class RoleQuery
{
    [UsePaging(MaxPageSize = 10)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<RoleDto>> GetMyRoles(
        [Service] IMediator mediator,
        QueryContext<RoleDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetMyRoles(offsetPagingArguments, rcontext),
            cancellationToken
        );
        return result.ToConnection();
    }

    [UsePaging(MaxPageSize = 10)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<RoleDto>> GetRoles(
        string broadcasterId,
        Roles.Enums.RoleType roleType,
        [Service] IMediator mediator,
        QueryContext<RoleDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetRoles(roleType, broadcasterId, offsetPagingArguments, rcontext),
            cancellationToken
        );
        return result.ToConnection();
    }

    public static async Task<RoleDto> GetMyRoleAsync(
        string broadcasterId,
        [FromServices] IMediator mediator
    )
    {
        return await mediator.Send(new GetMyRole(broadcasterId));
    }

    public static async Task<RoleDto> GetRoleAsync(Guid id, [FromServices] IMediator mediator)
    {
        return await mediator.Send(new GetRoleById(id));
    }
}
