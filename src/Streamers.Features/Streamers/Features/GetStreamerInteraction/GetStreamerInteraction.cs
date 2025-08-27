using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Followers.Models;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streams.Dtos;

namespace Streamers.Features.Streamers.Features.GetStreamerInteraction;

public record GetStreamerInteraction(string StreamerId) : IRequest<StreamerInteractionDto>;

public class GetStreamerInteractionHandler(
    ICurrentUser currentUser,
    StreamerDbContext streamerDbContext
) : IRequestHandler<GetStreamerInteraction, StreamerInteractionDto>
{
    public async Task<StreamerInteractionDto> Handle(
        GetStreamerInteraction request,
        CancellationToken cancellationToken
    )
    {
        if (!currentUser.IsAuthenticated)
        {
            return new StreamerInteractionDto { Followed = false, FollowedAt = null };
        }

        var interaction = new StreamerInteractionDto();
        Follower? follower = await streamerDbContext.Followers.FirstOrDefaultAsync(
            x => x.StreamerId == request.StreamerId && x.FollowerStreamerId == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (follower != null)
        {
            interaction.Followed = true;
            interaction.FollowedAt = follower.FollowedAt;
        }
        return interaction;
    }
}
