using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.ClearBan;

public record ClearBanResponse(Guid Id);

public record ClearBan(Guid Id) : IRequest<ClearBanResponse>;

public class ClearBanHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<ClearBan, ClearBanResponse>
{
    public async Task<ClearBanResponse> Handle(
        ClearBan request,
        CancellationToken cancellationToken
    )
    {
        var bannedUser = await streamerDbContext.BannedUsers.FirstOrDefaultAsync(
            x => x.Id == request.Id,
            cancellationToken: cancellationToken
        );
        if (bannedUser == null || bannedUser.BannedUntil > DateTime.UtcNow)
        {
            return new ClearBanResponse(request.Id);
        }
        bannedUser.Unban();
        streamerDbContext.BannedUsers.Remove(bannedUser);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new ClearBanResponse(request.Id);
    }
}
