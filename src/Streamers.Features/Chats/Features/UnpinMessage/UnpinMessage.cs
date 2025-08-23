using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Chats.Features.PinMessage;
using Streamers.Features.Chats.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.UnpinMessage;

public record UnpinMessageResponse(Guid MessageId);

public record UnpinMessage(Guid ChatId) : IRequest<UnpinMessageResponse>;

public class UnpinMessageHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<UnpinMessage, UnpinMessageResponse>
{
    public async Task<UnpinMessageResponse> Handle(
        UnpinMessage request,
        CancellationToken cancellationToken
    )
    {
        var streamer = await streamerDbContext.Streamers.FirstOrDefaultAsync(
            x => x.Id == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (streamer == null)
        {
            throw new InvalidOperationException(
                $"Could not find streamer with ID {currentUser.UserId}"
            );
        }
        var message = await streamerDbContext
            .PinnedChatMessages.Include(x => x.Message)
            .ThenInclude(x => x.Chat)
            .FirstOrDefaultAsync(
                x => x.Message.Chat.Id == request.ChatId,
                cancellationToken: cancellationToken
            );
        if (message == null)
        {
            throw new InvalidOperationException("Message not found");
        }
        message.Message.Chat.UnpinMessage();
        streamerDbContext.PinnedChatMessages.Remove(message);

        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new UnpinMessageResponse(message.Id);
    }
}
