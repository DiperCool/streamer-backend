using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Chats.Models;

public record ChatMessagePinnedEvent(string StreamerId, string ModeratorId, Guid ChatMessageId) : IDomainEvent;

public record ChatMessageUnpinnedEvent(string StreamerId, string ModeratorId, Guid ChatMessageId) : IDomainEvent;

public class PinnedChatMessage : Entity<Guid>
{
    protected PinnedChatMessage() { }

    public PinnedChatMessage(Guid id, ChatMessage message, Streamer pinnedBy, DateTime createdAt)
    {
        Id = id;
        MessageId = message.Id;
        Message = message;
        PinnedById = pinnedBy.Id;
        PinnedBy = pinnedBy;
        CreatedAt = createdAt;
    }

    public Guid MessageId { get; set; }
    public ChatMessage Message { get; set; } = default!;
    public string PinnedById { get; set; } = default!;
    public Streamer PinnedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public void RaisePinnedEvent(string streamerId)
    {
        Raise(new ChatMessagePinnedEvent(streamerId, PinnedById, MessageId));
    }

    public void RaiseUnpinnedEvent(string streamerId, string moderatorId)
    {
        Raise(new ChatMessageUnpinnedEvent(streamerId, moderatorId, MessageId));
    }
}
