using Microsoft.EntityFrameworkCore;
using Streamers.Features.Chats.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Services;

public class OnlyFollowerModeChatRule(StreamerDbContext streamerDbContext) : IChatRule
{
    public async Task<ChatRuleResponse> Check(ChatSettings chatSettings, string userId)
    {
        if (!chatSettings.FollowersOnly)
        {
            return new ChatRuleResponse(true, string.Empty);
        }

        var followed = await streamerDbContext.Followers.AnyAsync(x =>
            x.StreamerId == chatSettings.StreamerId && x.FollowerStreamerId == userId
        );
        if (followed)
        {
            return new ChatRuleResponse(true, string.Empty);
        }
        return new ChatRuleResponse(false, "User not followed");
    }
}
