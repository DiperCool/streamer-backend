using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;
using Streamers.Features.SystemRoles.Dtos;
using Streamers.Features.SystemRoles.Models;

namespace Streamers.Features.SystemRoles.Graphql;

[ObjectType<SystemRoleDto>]
public static partial class SystemRoleType
{
    public static async Task<StreamerDto?> GetStreamerAsync(
        [Parent(nameof(SystemRoleDto.StreamerId))] SystemRoleDto role,
        StreamersByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        return await dataLoader.LoadAsync(role.StreamerId, cancellationToken);
    }
}
