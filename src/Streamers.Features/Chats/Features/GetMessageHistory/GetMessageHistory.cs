using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.GetMessageHistory;

public record GetMessageHistory(
    Guid ChatId,
    DateTime StartFrom,
    QueryContext<ChatMessageDto> QueryContext
) : IRequest<List<ChatMessageDto>>;

public class GetMessageHistoryHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetMessageHistory, List<ChatMessageDto>>
{
    public async Task<List<ChatMessageDto>> Handle(
        GetMessageHistory request,
        CancellationToken cancellationToken
    )
    {
        var to = request.StartFrom.AddSeconds(5);
        var messages = await streamerDbContext
            .ChatMessages.AsNoTracking()
            .Where(x =>
                x.CreatedAt >= request.StartFrom && x.CreatedAt <= to && x.Chat.Id == request.ChatId
            )
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
            .ToListAsync(cancellationToken: cancellationToken);
        return messages;
    }
}
