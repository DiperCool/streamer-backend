using Streamers.Features.Streamers.Models;

namespace Streamers.Features.ModerationActivities.Models;

public class ChatModeAction : ModeratorActionType
{
    public string NewChatMode { get; protected set; } = default!; // Could be an enum

    public ChatModeAction(string moderatorId, string streamerId, string newChatMode)
        : base("ChatMode", moderatorId, streamerId)
    {
        NewChatMode = newChatMode;
    }

    private ChatModeAction()
        : base() { }
}
