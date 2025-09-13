using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Features.BanUser;
using Streamers.Features.Chats.Features.UnbanUser;

namespace Streamers.Features.Chats.GraphQl;

[MutationType]
public static partial class BannedUserMutations
{
    [Authorize]
    public static async Task<BanUserResponse> BanUser(BanUser request, [Service] IMediator mediator)
    {
        return await mediator.Send(request);
    }

    [Authorize]
    public static async Task<UnbanUserResponse> UnbanUser(
        UnbanUser request,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(request);
    }
}
