using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Streamers.Features.FinishAuth;
using Streamers.Features.Streamers.Features.UpdateAvatar;

namespace Streamers.Features.Streamers.GraphqQl;

[MutationType]
public static partial class StreamerMutation
{
    [Authorize]
    public static async Task<UpdateAvatarResponse> UpdateAvatar(
        UpdateAvatar input,
        IMediator mediator
    )
    {
        return await mediator.Send(input);
    }

    [Authorize]
    public static async Task<FinishAuthResponse> FinishAuth(FinishAuth input, IMediator mediator)
    {
        return await mediator.Send(input);
    }
}
