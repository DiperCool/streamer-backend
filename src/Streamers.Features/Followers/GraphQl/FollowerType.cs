using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Followers.Dtos;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;

namespace Streamers.Features.Followers.GraphQl;

[ObjectType<FollowerDto>]
public static partial class FollowerType
{
    public static async Task<StreamerDto?> GetFollowerStreamer(
        [Parent(nameof(FollowerDto.FollowerStreamerId))] FollowerDto follower,
        IStreamersByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        return await dataLoader.LoadAsync(follower.FollowerStreamerId, cancellationToken);
    }
}
