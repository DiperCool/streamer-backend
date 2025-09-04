using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.SystemRoles.Dtos;
using Streamers.Features.SystemRoles.Features.GetMySystemRole;

namespace Streamers.Features.SystemRoles.Graphql;

[QueryType]
public static partial class SystemRoleQuery
{
    [Authorize]
    public static async Task<SystemRoleDto> GetMySystemRoleAsync(IMediator mediator)
    {
        return await mediator.Send(new GetMySystemRole());
    }
}
