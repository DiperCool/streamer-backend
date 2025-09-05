using Microsoft.EntityFrameworkCore;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SystemRoles.Enums;

namespace Streamers.Features.SystemRoles.Services;

public interface ISystemRoleService
{
    Task<bool> HasAdministratorRole(string streamerId);
}

public class SystemRoleService(StreamerDbContext streamerDbContext) : ISystemRoleService
{
    public async Task<bool> HasAdministratorRole(string streamerId)
    {
        return await streamerDbContext.SystemRoles.AnyAsync(x =>
            x.StreamerId == streamerId && x.RoleType == SystemRoleType.Administrator
        );
    }
}
