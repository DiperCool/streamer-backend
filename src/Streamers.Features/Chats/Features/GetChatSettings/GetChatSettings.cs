using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.Exceptions;
using Streamers.Features.Chats.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.GetChatSettings;

public class GetChatSettings : IRequest<ChatSettingsDto>;

public class GetChatSettingsHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<GetChatSettings, ChatSettingsDto>
{
    public async Task<ChatSettingsDto> Handle(
        GetChatSettings request,
        CancellationToken cancellationToken
    )
    {
        ChatSettings? settings = await streamerDbContext.ChatSettings.FirstOrDefaultAsync(
            x => x.Streamer.Id == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (settings == null)
        {
            throw new ChatSettingsForStreamerNotFoundException(currentUser.UserId);
        }

        return new ChatSettingsDto
        {
            Id = settings.Id,
            SlowMode = settings.SlowMode,
            FollowersOnly = settings.FollowersOnly,
            SubscribersOnly = settings.SubscribersOnly,
            BannedWords = settings.BannedWords,
        };
    }
}
