using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Models;
using Streamers.Features.Shared.Persistance;

using Streamers.Features.ModerationActivities.Dtos;
using Streamers.Features.ModerationActivities.Services;

namespace Streamers.Features.ModerationActivities.Features.UnpinAction;

public class ChatMessageUnpinnedEventHandler(StreamerDbContext dbContext, IModerationActivityEventPublisher publisher)
    : IDomainEventHandler<ChatMessageUnpinnedEvent>
{
    public async Task Handle(
        ChatMessageUnpinnedEvent notification,
        CancellationToken cancellationToken
    )
    {
        var chatMessage = await dbContext.ChatMessages.Include(x => x.Chat)
            .FirstOrDefaultAsync(x => x.Id == notification.ChatMessageId, cancellationToken);

        if (chatMessage is null)
        {
            return;
        }

        var unpinAction = new Models.UnpinAction(
            moderatorId: notification.ModeratorId,
            streamerId: chatMessage.Chat.StreamerId,
            chatMessageId: notification.ChatMessageId
        );

        await dbContext.ModeratorActionTypes.AddAsync(unpinAction, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var unpinActionDto = new UnpinActionDto
        {
            Id = unpinAction.Id,
            Name = unpinAction.Name,
            ModeratorId = unpinAction.ModeratorId,
            StreamerId = unpinAction.StreamerId,
            CreatedDate = unpinAction.CreatedDate,
            ChatMessageId = unpinAction.ChatMessageId,
        };

        await publisher.PublishModerationActivityCreatedAsync(unpinActionDto, cancellationToken);
    }
}
