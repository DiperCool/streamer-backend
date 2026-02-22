using GreenDonut.Data;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.GetMessages;

public record GetMessages(
    Guid ChatId,
    PagingArguments Paging,
    QueryContext<ChatMessageDto> QueryContext
) : IRequest<Page<ChatMessageDto>>;

public class GetMessagesHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetMessages, Page<ChatMessageDto>>
{
    public async Task<Page<ChatMessageDto>> Handle(
        GetMessages request,
        CancellationToken cancellationToken
    )
    {
        Page<ChatMessageDto> result = await streamerDbContext
            .ChatMessages.Where(x => x.Chat.Id == request.ChatId)
            .Select(x => new ChatMessageDto
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Type = x.Type,
                SenderId = x.SenderId,
                Message = x.Message,
                IsDeleted = x.IsDeleted,
                IsActive = x.IsActive,
                ReplyId = x.ReplyId,
                IsUserSubscribed = x.IsUserSubscribed,
            })
            .With(request.QueryContext)
            .ToPageAsync(request.Paging, cancellationToken: cancellationToken);
        return result;
    }
}
