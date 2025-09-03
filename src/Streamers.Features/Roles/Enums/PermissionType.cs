namespace Streamers.Features.Roles.Enums;

[Flags]
public enum Permissions
{
    None = 0,
    Chat = 1 << 0,
    Stream = 1 << 1,
    Roles = 1 << 2,
    All = Chat | Stream | Roles,
}

public static class PermissionExtensions
{
    public static bool HasPermission(this Permissions permissions, Permissions permission)
    {
        return (permissions & permission) != 0;
    }
}
