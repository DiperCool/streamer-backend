using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Chats.Models;

public record ChatMessageCreated(ChatMessage ChatMessage) : IDomainEvent;

public record ChatMessageDeleted(ChatMessage ChatMessage) : IDomainEvent;

public class ChatMessage : Entity
{
    protected ChatMessage() { }

    public ChatMessage(
        ChatMessageType type,
        Streamer sender,
        string message,
        DateTime createdAt,
        ChatMessage? reply,
        Chat chat
    )
    {
        Sender = sender;
        Message = message;
        CreatedAt = createdAt;
        Type = type;
        Reply = reply;
        Chat = chat;
        Raise(new ChatMessageCreated(this));
    }

    public ChatMessageType Type { get; set; }
    public string SenderId { get; set; }
    public Streamer Sender { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }

    public ChatMessage? Reply { get; set; }
    public Guid? ReplyId { get; set; }
    public Chat Chat { get; set; }

    public void Remove()
    {
        IsDeleted = true;
        Raise(new ChatMessageDeleted(this));
    }
}
