using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Roles.Dtos;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;

namespace Streamers.Features.Roles.Graphql;

[ObjectType<RoleDto>]
public static partial class RoleType
{
    public static async Task<StreamerDto?> GetStreamer(
        [Parent(nameof(RoleDto.StreamerId))] RoleDto role,
        [Service] IStreamersByIdDataLoader streamersByIdDataLoader
    )
    {
        return await streamersByIdDataLoader.LoadAsync(role.StreamerId);
    }

    public static async Task<StreamerDto?> GetBroadcaster(
        [Parent(nameof(RoleDto.BroadcasterId))] RoleDto role,
        [Service] IStreamersByIdDataLoader streamersByIdDataLoader
    )
    {
        return await streamersByIdDataLoader.LoadAsync(role.BroadcasterId);
    }
}
