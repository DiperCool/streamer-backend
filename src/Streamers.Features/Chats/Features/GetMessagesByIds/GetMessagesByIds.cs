using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.GetMessagesByIds;

public record GetMessagesByIds(IReadOnlyList<Guid> Ids)
    : IRequest<IDictionary<Guid, ChatMessageDto>>;

public class GetMessagesByIdsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetMessagesByIds, IDictionary<Guid, ChatMessageDto>>
{
    public async Task<IDictionary<Guid, ChatMessageDto>> Handle(
        GetMessagesByIds request,
        CancellationToken cancellationToken
    )
    {
        var messages = await streamerDbContext
            .ChatMessages.Where(s => request.Ids.Contains(s.Id))
            .Select(s => new ChatMessageDto
            {
                Id = s.Id,
                CreatedAt = s.CreatedAt,
                Type = s.Type,
                SenderId = s.SenderId,
                Message = s.Message,
                IsDeleted = s.IsDeleted,
                IsActive = s.IsActive,
                ReplyId = s.ReplyId,
                IsUserSubscribed = s.IsUserSubscribed,
            })
            .ToListAsync(cancellationToken);

        var dict = messages.ToDictionary(s => s.Id, s => s);

        return dict;
    }
}
