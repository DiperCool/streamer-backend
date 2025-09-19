using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Notifications.Models;

public class UserFollowedNotification : Notification
{
    public UserFollowedNotification(string userId, DateTime createdAt, Streamer streamer)
        : base(userId, createdAt)
    {
        StreamerId = streamer.Id;
        Streamer = streamer;
    }

    protected UserFollowedNotification() { }

    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }
}
