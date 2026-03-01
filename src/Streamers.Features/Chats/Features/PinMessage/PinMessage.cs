using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Chats.Exceptions;
using Streamers.Features.Chats.Models;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Cqrs;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Exceptions;

namespace Streamers.Features.Chats.Features.PinMessage;

public record PinMessageResponse(Guid Id);

[Transactional]
public record PinMessage(Guid MessageId) : IRequest<PinMessageResponse>;

public class PinMessageHandle(
    StreamerDbContext streamerDbContext,
    IRoleService roleService,
    ICurrentUser currentUser
) : IRequestHandler<PinMessage, PinMessageResponse>
{
    public async Task<PinMessageResponse> Handle(
        PinMessage request,
        CancellationToken cancellationToken
    )
    {
        var streamer = await streamerDbContext.Streamers.FirstOrDefaultAsync(
            x => x.Id == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (streamer == null)
        {
            throw new StreamerNotFoundException(currentUser.UserId);
        }
        var message = await streamerDbContext
            .ChatMessages.Include(x => x.Chat)
            .ThenInclude(x => x.PinnedMessage)
            .FirstOrDefaultAsync(
                x => x.Id == request.MessageId,
                cancellationToken: cancellationToken
            );

        if (message == null)
        {
            throw new MessageNotFoundException(request.MessageId);
        }

        Chat chat = message.Chat;
        if (!await roleService.HasRole(chat.StreamerId, currentUser.UserId, Permissions.Chat))
        {
            throw new ForbiddenException();
        }
        if (chat.PinnedMessage != null)
        {
            streamerDbContext.PinnedChatMessages.Remove(chat.PinnedMessage);
        }
        chat.PinMessage(Guid.NewGuid(), message, streamer, DateTime.UtcNow);

        streamerDbContext.Chats.Update(chat);
        if (chat.PinnedMessage != null)
        {
            streamerDbContext.PinnedChatMessages.Add(chat.PinnedMessage);
        }
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new PinMessageResponse(message.Id);
    }
}
