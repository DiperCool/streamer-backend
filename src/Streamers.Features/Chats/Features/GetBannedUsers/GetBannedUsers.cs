using GreenDonut.Data;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.GetBannedUsers;

public record GetBannedUsers(
    string StreamerId,
    PagingArguments Paging,
    QueryContext<BannedUserDto> QueryContext
) : IRequest<Page<BannedUserDto>>;

public class GetBannedUsersHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetBannedUsers, Page<BannedUserDto>>
{
    public async Task<Page<BannedUserDto>> Handle(
        GetBannedUsers request,
        CancellationToken cancellationToken
    )
    {
        Page<BannedUserDto> result = await streamerDbContext
            .BannedUsers.Where(x => x.BroadcasterId == request.StreamerId)
            .Select(x => new BannedUserDto
            {
                Id = x.Id,
                UserId = x.UserId,
                BannedById = x.BannedById,
                BannedAt = x.BannedAt,
                BannedUntil = x.BannedUntil,
                Reason = x.Reason,
            })
            .With(request.QueryContext)
            .ToPageAsync(request.Paging, cancellationToken: cancellationToken);
        return result;
    }
}
