using HotChocolate.Subscriptions;
using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.Models;

namespace Streamers.Features.Chats.Features.CreateMessage;

public class ChatMessageCreatedEventHandler(ITopicEventSender sender)
    : IDomainEventHandler<ChatMessageCreated>
{
    public async Task Handle(ChatMessageCreated domainEvent, CancellationToken cancellationToken)
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
            $"{nameof(ChatMessageCreated)}-{message.Chat.Id}",
            dto,
            cancellationToken
        );
    }
}
