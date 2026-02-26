using Streamers.Features.Streamers.Models;

namespace Streamers.Features.ModerationActivities.Models;

public class UnbanAction : ModeratorActionType
{
    public string TargetUserId { get; protected set; } = default!;
    public Streamer TargetUser { get; protected set; } = default!;

    public UnbanAction(string moderatorId, string streamerId, string targetUserId)
        : base("Unban", moderatorId, streamerId)
    {
        TargetUserId = targetUserId;
    }

    private UnbanAction()
        : base() { }
}
