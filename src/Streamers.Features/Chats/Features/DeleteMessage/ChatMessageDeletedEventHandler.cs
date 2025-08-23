using HotChocolate.Subscriptions;
using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.Models;

namespace Streamers.Features.Chats.Features.DeleteMessage;

public class ChatMessageDeletedEventHandler(ITopicEventSender sender)
    : IDomainEventHandler<ChatMessageDeleted>
{
    public async Task Handle(ChatMessageDeleted domainEvent, CancellationToken cancellationToken)
    {
        var message = domainEvent.ChatMessage;
        var dto = new ChatMessageDto
        {
            Id = message.Id,
            Type = message.Type,
            SenderId = message.SenderId,
            Message = message.Message,
            CreatedAt = message.CreatedAt,
            IsDeleted = message.IsDeleted,
            IsActive = message.IsActive,
            ReplyId = message.ReplyId,
        };
        await sender.SendAsync(
            $"{nameof(ChatMessageDeleted)}-{message.Chat.Id}",
            dto,
            cancellationToken
        );
    }
}
