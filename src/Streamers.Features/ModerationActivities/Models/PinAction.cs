using Streamers.Features.Chats.Models;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.ModerationActivities.Models;

public class PinAction : ModeratorActionType
{
    public Guid ChatMessageId { get; protected set; }
    public ChatMessage ChatMessage { get; protected set; } = default!;

    public PinAction(string moderatorId, string streamerId, Guid chatMessageId)
        : base("Pin", moderatorId, streamerId)
    {
        ChatMessageId = chatMessageId;
    }

    private PinAction()
        : base() { }
}
