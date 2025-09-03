using Streamers.Features.Roles.Enums;

namespace Streamers.Features.Roles.Dtos;

public class RoleDto
{
    public required Guid Id { get; set; }

    public required string StreamerId { get; set; }
    public required RoleType Type { get; set; }
    public required string BroadcasterId { get; set; }
    public required Permissions Permissions { get; set; }
}
