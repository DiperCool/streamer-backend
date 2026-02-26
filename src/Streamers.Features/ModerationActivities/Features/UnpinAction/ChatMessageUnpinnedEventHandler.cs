using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.ModerationActivities.Features.UnpinAction;

public class ChatMessageUnpinnedEventHandler(StreamerDbContext dbContext)
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
    }
}
