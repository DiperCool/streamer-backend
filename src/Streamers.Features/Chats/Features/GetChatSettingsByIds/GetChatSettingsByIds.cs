using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.GetChatSettingsByIds;

public record GetChatSettingsByIds(IReadOnlyList<Guid> Ids)
    : IRequest<IDictionary<Guid, ChatSettingsDto>>;

public class GetChatSettingsByIdsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetChatSettingsByIds, IDictionary<Guid, ChatSettingsDto>>
{
    public async Task<IDictionary<Guid, ChatSettingsDto>> Handle(
        GetChatSettingsByIds request,
        CancellationToken cancellationToken
    )
    {
        var chats = await streamerDbContext
            .ChatSettings.Where(s => request.Ids.Contains(s.Id))
            .Select(s => new ChatSettingsDto
            {
                Id = s.Id,
                SlowMode = s.SlowMode,
                FollowersOnly = s.FollowersOnly,
                SubscribersOnly = s.SubscribersOnly,
                BannedWords = s.BannedWords,
            })
            .ToListAsync(cancellationToken);

        var dict = chats.ToDictionary(s => s.Id, s => s);

        return dict;
    }
}
