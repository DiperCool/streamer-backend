using HotChocolate.Subscriptions;
using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.Models;

namespace Streamers.Features.Chats.Features.UpdateChatSettings;

public class ChatUpdatedEventHandler(ITopicEventSender sender) : IDomainEventHandler<ChatUpdated>
{
    public async Task Handle(ChatUpdated domainEvent, CancellationToken cancellationToken)
    {
        var chat = domainEvent.Chat;
        var dto = new ChatDto
        {
            SettingsId = chat.SettingsId,
            PinnedMessageId = chat.SettingsId,
            StreamerId = chat.StreamerId,
            Id = chat.Id,
        };
        await sender.SendAsync($"{nameof(ChatUpdated)}-{chat.Id}", dto, cancellationToken);
    }
}
