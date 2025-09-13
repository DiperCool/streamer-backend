using Streamers.Features.Chats.Models;

namespace Streamers.Features.Chats.Services;

public record ChatRuleResponse(bool Response, string Message);

public interface IChatRule
{
    Task<ChatRuleResponse> Check(ChatSettings chatSettings, string userId);
}
