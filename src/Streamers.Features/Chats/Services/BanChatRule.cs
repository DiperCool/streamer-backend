using Microsoft.EntityFrameworkCore;
using Streamers.Features.Chats.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Services;

public class BanChatRule(StreamerDbContext streamerDbContext) : IChatRule
{
    public async Task<ChatRuleResponse> Check(ChatSettings chatSettings, string userId)
    {
        if (
            await streamerDbContext.BannedUsers.AnyAsync(x =>
                x.BroadcasterId == chatSettings.StreamerId && x.UserId == userId
            )
        )
        {
            return new ChatRuleResponse(false, "User banned");
        }

        return new ChatRuleResponse(true, string.Empty);
    }
}
