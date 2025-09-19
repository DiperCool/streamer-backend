using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Notifications.Models;

public class NotificationSettings : Entity
{
    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }
    public bool StreamerLive { get; set; }
    public bool UserFollowed { get; set; }
}
