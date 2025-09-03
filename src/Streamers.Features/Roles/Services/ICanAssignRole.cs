using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Models;

namespace Streamers.Features.Roles.Services;

public interface ICanAssignRole
{
    void EnsureCanAssign(Role creatorRole, RoleType targetRoleType, Permissions targetPermissions);
}

public class CanAssignRole(IRolesHierarchy rolesHierarchy) : ICanAssignRole
{
    private static readonly Dictionary<RoleType, Permissions> AssignablePermissions = new()
    {
        { RoleType.Administrator, Permissions.Chat | Permissions.Stream },
        { RoleType.Broadcaster, Permissions.All },
    };

    public void EnsureCanAssign(
        Role creatorRole,
        RoleType targetRoleType,
        Permissions targetPermissions
    )
    {
        if (creatorRole == null)
            throw new UnauthorizedAccessException("You do not have a role in this broadcaster.");

        if (!rolesHierarchy.CanAssign(creatorRole.Type, targetRoleType))
            throw new UnauthorizedAccessException(
                $"Your role '{creatorRole.Type}' cannot assign role '{targetRoleType}'."
            );

        if (!AssignablePermissions.TryGetValue(creatorRole.Type, out var allowed))
            throw new UnauthorizedAccessException("You cannot assign any permissions.");

        if ((targetPermissions & allowed) != targetPermissions)
            throw new UnauthorizedAccessException(
                "You are trying to assign permissions you are not allowed to."
            );

        if (
            creatorRole.Type == RoleType.Administrator
            && targetPermissions.HasPermission(Permissions.Roles)
        )
            throw new UnauthorizedAccessException("Administrators cannot assign Roles permission.");
    }
}
