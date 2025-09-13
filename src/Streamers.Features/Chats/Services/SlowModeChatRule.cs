using Microsoft.EntityFrameworkCore;
using Streamers.Features.Chats.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Services;

public class SlowModeChatRule(StreamerDbContext streamerDbContext) : IChatRule
{
    public async Task<ChatRuleResponse> Check(ChatSettings chatSettings, string userId)
    {
        if (chatSettings.SlowMode != null)
        {
            return new ChatRuleResponse(true, string.Empty);
        }
        var lastTimeMessage = await streamerDbContext
            .ChatMessages.Where(x =>
                x.Chat.StreamerId == chatSettings.StreamerId && x.SenderId == userId
            )
            .Select(x => (DateTime?)x.CreatedAt)
            .FirstOrDefaultAsync();
        if (lastTimeMessage == null)
        {
            return new ChatRuleResponse(true, string.Empty);
        }

        if (lastTimeMessage.Value.AddSeconds(chatSettings.SlowMode ?? 0) < DateTime.UtcNow)
        {
            return new ChatRuleResponse(true, string.Empty);
        }
        return new ChatRuleResponse(false, "Slow mode");
    }
}
