using Microsoft.EntityFrameworkCore;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Roles.Services;

public interface IRoleService
{
    Task<bool> HasRole(string broadcasterId, string userId, Permissions permissions);
}

public class RoleService(StreamerDbContext streamerDbContext) : IRoleService
{
    public async Task<bool> HasRole(string broadcasterId, string userId, Permissions permissions)
    {
        var role = await streamerDbContext.Roles.FirstOrDefaultAsync(x =>
            x.StreamerId == broadcasterId
        );
        if (role == null)
        {
            return false;
        }

        if (!role.Permissions.HasPermission(permissions))
        {
            return false;
        }
        return true;
    }
}
