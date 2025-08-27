using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.Features.GetEmail;
using Streamers.Features.Streamers.Features.GetStreamer;
using Streamers.Features.Streamers.Features.GetStreamerByUserName;
using Streamers.Features.Streamers.Features.GetStreamerInteraction;
using Streamers.Features.Streams.Dtos;

namespace Streamers.Features.Streamers.GraphqQl;

[QueryType]
public static partial class StreamerQuery
{
    [Authorize]
    public static async Task<StreamerMeDto> GetMeAsync([Service] IMediator mediator)
    {
        var response = await mediator.Send(new GetStreamer());
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

    public static async Task<StreamerDto> GetStreamerAsync(
        string userName,
        [Service] IMediator mediator
    )
    {
        var response = await mediator.Send(new GetStreamerByUserName(userName));
        return response;
    }

    public static async Task<StreamerInteractionDto> GetStreamerInteractionAsync(
        string streamerId,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(new GetStreamerInteraction(streamerId));
    }
}
