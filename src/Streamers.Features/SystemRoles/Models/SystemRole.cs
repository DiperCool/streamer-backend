using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;
using Streamers.Features.SystemRoles.Enums;

namespace Streamers.Features.SystemRoles.Models;

public class SystemRole : Entity
{
    public SystemRole(Streamer streamer, SystemRoleType roleType)
    {
        Streamer = streamer;
        RoleType = roleType;
    }

    protected SystemRole() { }

    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }
    public SystemRoleType RoleType { get; set; }
}
