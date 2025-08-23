using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Chats.Models;

public class PinnedChatMessage : Entity
{
    protected PinnedChatMessage() { }

    public PinnedChatMessage(Guid id, ChatMessage message, Streamer pinnedBy, DateTime createdAt)
    {
        Id = id;
        Message = message;
        PinnedBy = pinnedBy;
        CreatedAt = createdAt;
    }

    public Guid MessageId { get; set; }
    public ChatMessage Message { get; set; }
    public string PinnedById { get; set; }
    public Streamer PinnedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
