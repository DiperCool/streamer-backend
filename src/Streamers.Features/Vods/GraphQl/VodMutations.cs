using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Vods.Features.RemoveVod;
using Streamers.Features.Vods.Features.UpdateVod;

namespace Streamers.Features.Vods.GraphQl;

[MutationType]
[Authorize]
public static partial class VodMutations
{
    public static async Task<UpdateVodResponse> UpdateVod(UpdateVod request, IMediator mediator)
    {
        return await mediator.Send(request);
    }

    public static async Task<RemoveVodResponse> RemoveVod(RemoveVod request, IMediator mediator)
    {
        return await mediator.Send(request);
    }
}
