using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.ModerationActivities.Features.PinAction;

public class ChatMessagePinnedEventHandler(StreamerDbContext dbContext)
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
    }
}
