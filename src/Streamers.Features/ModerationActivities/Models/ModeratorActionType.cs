using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.ModerationActivities.Models;

public abstract class ModeratorActionType : Entity
{
    public string Name { get; protected set; }

    public string ModeratorId { get; protected set; } = default!;
    public Streamer Moderator { get; protected set; } = default!;

    public string StreamerId { get; protected set; } = default!;
    public Streamer Streamer { get; protected set; } = default!;

    protected ModeratorActionType(string name, string moderatorId, string streamerId)
    {
        Id = Guid.NewGuid();
        Name = name;
        ModeratorId = moderatorId;
        StreamerId = streamerId;
    }

    // Parameterless constructor for EF Core
    protected ModeratorActionType() { }
}
