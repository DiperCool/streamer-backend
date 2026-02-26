using Streamers.Features.Streamers.Models;
using Streamers.Features.Chats.Models; // New using

namespace Streamers.Features.ModerationActivities.Models;

public class UnpinAction : ModeratorActionType
{
    public Guid ChatMessageId { get; protected set; } // Renamed from MessageId
    public ChatMessage ChatMessage { get; protected set; } = default!; // Navigation property

    public UnpinAction(string moderatorId, string streamerId, Guid chatMessageId)
        : base("Unpin", moderatorId, streamerId)
    {
        ChatMessageId = chatMessageId;
    }

    private UnpinAction()
        : base() { }
}
