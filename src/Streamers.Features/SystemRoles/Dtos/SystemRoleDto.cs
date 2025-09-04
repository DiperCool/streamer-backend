using Streamers.Features.SystemRoles.Enums;

namespace Streamers.Features.SystemRoles.Dtos;

public class SystemRoleDto
{
    public required string StreamerId { get; set; }
    public required SystemRoleType RoleType { get; set; }
}
