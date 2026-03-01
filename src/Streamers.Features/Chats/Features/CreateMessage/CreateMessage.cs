using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Chats.Enums;
using Streamers.Features.Chats.Exceptions;
using Streamers.Features.Chats.Models;
using Streamers.Features.Chats.Services;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Exceptions;

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

public class CreateMessageHandler(
    StreamerDbContext streamerDbContext,
    IChatPermissionService permissionService,
    ICurrentUser currentUser
) : IRequestHandler<CreateMessage, CreateMessageResponse>
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
            throw new StreamerNotFoundException(currentUser.UserId);
        }
        var chat = await streamerDbContext
            .Chats.Include(x => x.Settings)
            .FirstOrDefaultAsync(x => x.Id == request.ChatId, cancellationToken: cancellationToken);
        if (chat == null)
        {
            throw new ChatNotFoundException(request.ChatId);
        }
        var chatPermissions = await permissionService.Check(chat.Settings, currentUser.UserId);
        if (chatPermissions != null)
        {
            throw new ForbiddenException(chatPermissions.Message);
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
            throw new MessageNotFoundException(request.ReplyMessageId.Value);
        }

        var subscription = await streamerDbContext.Subscriptions.AnyAsync(
            x => x.UserId == currentUser.UserId && x.StreamerId == chat.StreamerId,
            cancellationToken: cancellationToken
        );
        var chatMessage = new ChatMessage(
            ChatMessageType.UserMessage,
            streamer,
            request.Message,
            DateTime.UtcNow,
            reply,
            chat,
            subscription
        );
        await streamerDbContext.ChatMessages.AddAsync(chatMessage, cancellationToken);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new CreateMessageResponse(chatMessage.Id);
    }
}
