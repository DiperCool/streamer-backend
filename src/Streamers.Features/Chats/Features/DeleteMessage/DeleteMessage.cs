using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.DeleteMessage;

public record DeleteMessageResponse(Guid Id);

public record DeleteMessage(Guid MessageId) : IRequest<DeleteMessageResponse>;

public class DeleteMessageHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser,
    IRoleService roleService
) : IRequestHandler<DeleteMessage, DeleteMessageResponse>
{
    public async Task<DeleteMessageResponse> Handle(
        DeleteMessage request,
        CancellationToken cancellationToken
    )
    {
        var message = await streamerDbContext
            .ChatMessages.Include(x => x.Chat)
            .FirstOrDefaultAsync(
                x => x.Id == request.MessageId,
                cancellationToken: cancellationToken
            );

        if (message == null)
        {
            throw new InvalidOperationException("Message not found");
        }
        if (
            !await roleService.HasRole(
                message.Chat.StreamerId,
                currentUser.UserId,
                Permissions.Chat
            )
        )
        {
            throw new UnauthorizedAccessException();
        }
        message.Remove();
        streamerDbContext.ChatMessages.Update(message);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new DeleteMessageResponse(message.Id);
    }
}
