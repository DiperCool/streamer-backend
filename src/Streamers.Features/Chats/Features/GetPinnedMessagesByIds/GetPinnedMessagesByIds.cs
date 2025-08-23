using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.GetPinnedMessagesByIds;

public record GetPinnedMessagesByIds(IReadOnlyList<Guid> Ids)
    : IRequest<IDictionary<Guid, PinnedChatMessageDto>>;

public class GetPinnedMessagesByIdsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetPinnedMessagesByIds, IDictionary<Guid, PinnedChatMessageDto>>
{
    public async Task<IDictionary<Guid, PinnedChatMessageDto>> Handle(
        GetPinnedMessagesByIds request,
        CancellationToken cancellationToken
    )
    {
        var pinnedMessages = await streamerDbContext
            .PinnedChatMessages.Where(s => request.Ids.Contains(s.Id))
            .Select(s => new PinnedChatMessageDto
            {
                Id = s.Id,
                MessageId = s.MessageId,
                PinnedById = s.PinnedById,
                CreatedAt = s.CreatedAt,
            })
            .ToListAsync(cancellationToken);

        var dict = pinnedMessages.ToDictionary(s => s.Id, s => s);

        return dict;
    }
}
