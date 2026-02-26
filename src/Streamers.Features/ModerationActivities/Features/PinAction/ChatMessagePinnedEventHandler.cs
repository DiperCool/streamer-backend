using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Models;
using Streamers.Features.Shared.Persistance;

using Streamers.Features.ModerationActivities.Dtos;
using Streamers.Features.ModerationActivities.Services;

namespace Streamers.Features.ModerationActivities.Features.PinAction;

public class ChatMessagePinnedEventHandler(StreamerDbContext dbContext, IModerationActivityEventPublisher publisher)
    : IDomainEventHandler<ChatMessagePinnedEvent>
{
    public async Task Handle(
        ChatMessagePinnedEvent notification,
        CancellationToken cancellationToken
    )
    {
        var chatMessage = await dbContext.ChatMessages.Include(x => x.Chat)
            .FirstOrDefaultAsync(x => x.Id == notification.ChatMessageId, cancellationToken);

        if (chatMessage is null)
        {
            return;
        }

        var pinAction = new Models.PinAction(
            moderatorId: notification.ModeratorId,
            streamerId: chatMessage.Chat.StreamerId,
            chatMessageId: notification.ChatMessageId
        );

        await dbContext.ModeratorActionTypes.AddAsync(pinAction, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var pinActionDto = new PinActionDto
        {
            Id = pinAction.Id,
            Name = pinAction.Name,
            ModeratorId = pinAction.ModeratorId,
            StreamerId = pinAction.StreamerId,
            CreatedDate = pinAction.CreatedDate,
            ChatMessageId = pinAction.ChatMessageId,
        };

        await publisher.PublishModerationActivityCreatedAsync(pinActionDto, cancellationToken);
    }
}
