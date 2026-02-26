using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Chats.Models;

public record ChatUpdated(Chat Chat) : IDomainEvent;

public class Chat : Entity
{
    protected Chat() { }

    public Chat(ChatSettings settings)
    {
        Settings = settings;
    }

    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }
    public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

    public Guid? PinnedMessageId { get; set; }
    public PinnedChatMessage? PinnedMessage { get; set; }
    public Guid SettingsId { get; set; }
    public ChatSettings Settings { get; set; }

    public void PinMessage(Guid id, ChatMessage message, Streamer streamer, DateTime dateTime)
    {
        PinnedMessage = new PinnedChatMessage(id, message, streamer, dateTime);
        PinnedMessageId = id;
        PinnedMessage.RaisePinnedEvent(StreamerId);
        Raise(new ChatUpdated(this));
    }

    public void UnpinMessage(string moderatorId)
    {
        if (PinnedMessage is null)
            return;

        var pinnedBy = PinnedMessage.PinnedById;
        PinnedMessage.RaiseUnpinnedEvent(StreamerId, moderatorId);

        PinnedMessage = null;
        PinnedMessageId = null;
        Raise(new ChatUpdated(this));
    }
}
