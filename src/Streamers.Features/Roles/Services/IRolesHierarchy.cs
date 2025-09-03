using Streamers.Features.Roles.Enums;

namespace Streamers.Features.Roles.Services;

public interface IRolesHierarchy
{
    bool CanAssign(RoleType creator, RoleType target);
}

public class RolesHierarchy : IRolesHierarchy
{
    private static readonly Dictionary<RoleType, int> RoleLevels = new()
    {
        { RoleType.Broadcaster, 2 },
        { RoleType.Administrator, 1 },
    };

    public bool CanAssign(RoleType creator, RoleType target)
    {
        return RoleLevels.TryGetValue(creator, out var creatorLevel)
            && RoleLevels.TryGetValue(target, out var targetLevel)
            && creatorLevel > targetLevel;
    }
}
