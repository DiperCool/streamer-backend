using Streamers.Features.Roles.Enums;
using Streamers.Features.Subscriptions.Dtos;

namespace Streamers.Features.Streams.Dtos;

public class StreamerInteractionDto
{
    public bool Followed { get; set; }
    public DateTime? FollowedAt { get; set; }

    public bool Banned { get; set; }
    public DateTime? BannedUntil { get; set; }

    public Permissions Permissions { get; set; }

    public DateTime? LastTimeMessage { get; set; }

    public SubscriptionDto Subscription { get; set; }
}
