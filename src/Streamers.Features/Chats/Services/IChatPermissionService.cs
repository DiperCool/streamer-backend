using Streamers.Features.Chats.Models;

namespace Streamers.Features.Chats.Services;

public interface IChatPermissionService
{
    public Task<ChatRuleResponse?> Check(ChatSettings chatSettings, string userId);
}

public class ChatPermissionService(IEnumerable<IChatRule> rules) : IChatPermissionService
{
    public async Task<ChatRuleResponse?> Check(ChatSettings chatSettings, string userId)
    {
        foreach (var rule in rules)
        {
            var response = await rule.Check(chatSettings, userId);
            if (!response.Response)
            {
                return response;
            }
        }
        return null;
    }
}
