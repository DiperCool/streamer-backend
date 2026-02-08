using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Chats.Dtos;

namespace Streamers.Features.Chats.GraphQl;

[SubscriptionType]
public static partial class ChatMessageSubscription
{
    [Subscribe]
    [Topic($"{nameof(ChatMessageCreated)}-{{{nameof(chatId)}}}")]
    public static ChatMessageDto ChatMessageCreated(
        Guid chatId,
        [EventMessage] ChatMessageDto message
    ) => message;

    [Subscribe]
    [Topic($"{nameof(ChatMessageDeleted)}-{{{nameof(chatId)}}}")]
    public static ChatMessageDto ChatMessageDeleted(
        Guid chatId,
        [EventMessage] ChatMessageDto message
    ) => message;

    [Subscribe]
    [Topic($"{nameof(ChatUpdated)}-{{{nameof(chatId)}}}")]
    public static ChatDto ChatUpdated(Guid chatId, [EventMessage] ChatDto chat) => chat;
}
