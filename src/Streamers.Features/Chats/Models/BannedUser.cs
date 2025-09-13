using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Chats.Models;

public record UserBanned(BannedUser BannedUser) : IDomainEvent;

public record UserUnbanned(BannedUser BannedUser) : IDomainEvent;

public class BannedUser : Entity
{
    protected BannedUser() { }

    public BannedUser(
        Streamer user,
        Streamer broadcaster,
        Streamer bannedBy,
        DateTime bannedAt,
        DateTime bannedUntil,
        string reason
    )
    {
        UserId = user.Id;
        User = user;
        BroadcasterId = broadcaster.Id;
        Broadcaster = broadcaster;
        BannedById = bannedBy.Id;

        BannedBy = bannedBy;
        BannedAt = bannedAt;
        BannedUntil = bannedUntil;
        Reason = reason;
        Raise(new UserBanned(this));
    }

    public string UserId { get; set; }
    public Streamer User { get; set; }

    public string BroadcasterId { get; set; }
    public Streamer Broadcaster { get; set; }

    public string BannedById { get; set; }
    public Streamer BannedBy { get; set; }
    public DateTime BannedAt { get; set; }
    public DateTime BannedUntil { get; set; }
    public string JobId { get; set; }
    public string Reason { get; set; }

    public void Unban()
    {
        Raise(new UserUnbanned(this));
    }
}
