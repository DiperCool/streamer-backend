using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.DeleteMessage;

public record DeleteMessageResponse(Guid Id);

public record DeleteMessage(Guid MessageId) : IRequest<DeleteMessageResponse>;

public class DeleteMessageHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<DeleteMessage, DeleteMessageResponse>
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
        message.Remove();
        streamerDbContext.ChatMessages.Update(message);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new DeleteMessageResponse(message.Id);
    }
}
