using Shared.Abstractions.Domain;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Roles.Models;

public class Role : Entity
{
    public Role(
        Streamer streamer,
        RoleType type,
        Streamer broadcaster,
        DateTime createdAt,
        Permissions permissions
    )
    {
        Streamer = streamer;
        Type = type;
        Broadcaster = broadcaster;
        CreatedAt = createdAt;
        Permissions = permissions;
    }

    private Role() { }

    public Permissions Permissions { get; set; }
    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }
    public RoleType Type { get; set; }
    public string BroadcasterId { get; set; }
    public Streamer Broadcaster { get; set; }
    public DateTime CreatedAt { get; set; }
}
