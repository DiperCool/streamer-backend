using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Roles.Features.CreateRole;
using Streamers.Features.Roles.Features.EditRole;
using Streamers.Features.Roles.Features.RemoveRole;

namespace Streamers.Features.Roles.Graphql;

[MutationType]
public static partial class RoleMutations
{
    [Authorize]
    public static async Task<CreateRoleResponse> CreateRole(CreateRole input, IMediator mediator)
    {
        return await mediator.Send(input);
    }

    [Authorize]
    public static async Task<RemoveRoleResponse> RemoveRole(RemoveRole input, IMediator mediator)
    {
        return await mediator.Send(input);
    }

    [Authorize]
    public static async Task<EditRoleResponse> EditRole(EditRole input, IMediator mediator)
    {
        return await mediator.Send(input);
    }
}
