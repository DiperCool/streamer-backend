using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Followers.Features.Follow;
using Streamers.Features.Followers.Features.Unfollow;

namespace Streamers.Features.Followers.GraphQl;

[MutationType]
public static partial class FollowersMutations
{
    [Authorize]
    public static async Task<FollowResponse> Follow(Follow follow, [Service] IMediator mediator)
    {
        return await mediator.Send(follow);
    }

    [Authorize]
    public static async Task<UnfollowResponse> Unfollow(
        Unfollow unfollow,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(unfollow);
    }
}
