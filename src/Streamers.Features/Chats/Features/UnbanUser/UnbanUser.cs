using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.UnbanUser;

public record UnbanUserResponse(Guid Id);

public record UnbanUser(string BroadcasterId, string UserId) : IRequest<UnbanUserResponse>;

public class UnbanUserHandler(
    StreamerDbContext streamerDbContext,
    IRoleService roleService,
    ICurrentUser currentUser
) : IRequestHandler<UnbanUser, UnbanUserResponse>
{
    public async Task<UnbanUserResponse> Handle(
        UnbanUser request,
        CancellationToken cancellationToken
    )
    {
        if (!await roleService.HasRole(request.BroadcasterId, currentUser.UserId, Permissions.Chat))
        {
            throw new UnauthorizedAccessException();
        }

        var bannedUser = await streamerDbContext.BannedUsers.FirstOrDefaultAsync(
            x => x.UserId == request.UserId && x.BroadcasterId == request.BroadcasterId,
            cancellationToken: cancellationToken
        );
        if (bannedUser == null)
        {
            throw new InvalidOperationException("You do not have permission to use this command");
        }
        bannedUser.Unban();
        streamerDbContext.BannedUsers.Remove(bannedUser);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new UnbanUserResponse(bannedUser.Id);
    }
}
