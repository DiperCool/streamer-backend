using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.Features.GetEmail;
using Streamers.Features.Streamers.Features.GetStreamer;
using Streamers.Features.Streamers.Features.GetStreamerByUserName;

namespace Streamers.Features.Streamers.GraphqQl;

[QueryType]
public static partial class StreamerQuery
{
    [Authorize]
    public static async Task<StreamerDto> GetMeAsync(
        [Service] IMediator mediator,
        [Service] ICurrentUser currentUser
    )
    {
        var response = await mediator.Send(new GetStreamer(currentUser.UserId));
        return response;
    }

    [Authorize]
    public static async Task<GetEmailResponse> GetMyEmailAsync(
        [Service] IMediator mediator,
        [Service] ICurrentUser currentUser
    )
    {
        var response = await mediator.Send(new GetEmail(currentUser.UserId));
        return response;
    }

    public static async Task<StreamerDto> GetStreamer(string userName, [Service] IMediator mediator)
    {
        var response = await mediator.Send(new GetStreamerByUserName(userName));
        return response;
    }
}
