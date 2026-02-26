using Streamers.Features.Streamers.Models;

namespace Streamers.Features.ModerationActivities.Models;

public class BanAction : ModeratorActionType
{
    public string TargetUserId { get; protected set; } = default!;
    public Streamer TargetUser { get; protected set; } = default!;

    public DateTime BannedUntil { get; protected set; }
    public string Reason { get; protected set; } = default!;

    public BanAction(
        string moderatorId,
        string streamerId,
        string targetUserId,
        DateTime bannedUntil,
        string reason
    )
        : base("Ban", moderatorId, streamerId)
    {
        TargetUserId = targetUserId;
        BannedUntil = bannedUntil;
        Reason = reason;
    }

    private BanAction()
        : base() { }
}
