using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Chats.Enums;
using Streamers.Features.Chats.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.CreateMessage;

public record CreateMessageResponse(Guid MessageId);

public record CreateMessage(Guid ChatId, string Message, Guid? ReplyMessageId)
    : IRequest<CreateMessageResponse>;

public class CreateMessageValidator : AbstractValidator<CreateMessage>
{
    public CreateMessageValidator()
    {
        RuleFor(x => x.Message).NotEmpty().MaximumLength(250);
    }
}

public class CreateMessageHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<CreateMessage, CreateMessageResponse>
{
    public async Task<CreateMessageResponse> Handle(
        CreateMessage request,
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
        var chat = await streamerDbContext.Chats.FirstOrDefaultAsync(
            x => x.Id == request.ChatId,
            cancellationToken: cancellationToken
        );
        if (chat == null)
        {
            throw new InvalidOperationException($"Could not find chat with ID {request.ChatId}");
        }

        var reply =
            request.ReplyMessageId == null
                ? null
                : await streamerDbContext.ChatMessages.FirstOrDefaultAsync(
                    x => x.Id == request.ReplyMessageId && x.Chat.Id == request.ChatId,
                    cancellationToken: cancellationToken
                );
        if (reply == null && request.ReplyMessageId != null)
        {
            throw new InvalidOperationException(
                $"Could not find chat message with ID {request.ReplyMessageId}"
            );
        }
        var chatMessage = new ChatMessage(
            ChatMessageType.UserMessage,
            streamer,
            request.Message,
            DateTime.UtcNow,
            reply,
            chat
        );
        await streamerDbContext.ChatMessages.AddAsync(chatMessage, cancellationToken);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new CreateMessageResponse(chatMessage.Id);
    }
}
