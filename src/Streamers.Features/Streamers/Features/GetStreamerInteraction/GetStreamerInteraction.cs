using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Chats.Models;
using Streamers.Features.Followers.Models;
using Streamers.Features.Roles.Enums;
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
        var chatSettings = await streamerDbContext.ChatSettings.FirstOrDefaultAsync(
            x => x.StreamerId == request.StreamerId,
            cancellationToken: cancellationToken
        );
        if (chatSettings == null)
        {
            throw new InvalidOperationException("Chat settings not found");
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

        BannedUser? bannedUser = await streamerDbContext.BannedUsers.FirstOrDefaultAsync(
            x => x.BroadcasterId == request.StreamerId && x.UserId == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (bannedUser != null)
        {
            interaction.Banned = true;
            interaction.BannedUntil = bannedUser.BannedUntil;
        }

        var role = await streamerDbContext.Roles.FirstOrDefaultAsync(
            x => x.BroadcasterId == request.StreamerId && x.StreamerId == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        var lastTimeMessage = await streamerDbContext
            .ChatMessages.Where(x =>
                x.Chat.StreamerId == chatSettings.StreamerId && x.SenderId == currentUser.UserId
            )
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => (DateTime?)x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (lastTimeMessage != null && chatSettings.SlowMode != null)
        {
            interaction.LastTimeMessage = lastTimeMessage;
        }
        interaction.Permissions = role?.Permissions ?? Permissions.None;
        return interaction;
    }
}
