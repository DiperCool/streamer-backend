using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Notifications.Models;

public class LiveStartedNotification : Notification
{
    public LiveStartedNotification(string userId, DateTime createdAt, Streamer streamer)
        : base(userId, createdAt)
    {
        StreamerId = streamer.Id;
        Streamer = streamer;
    }

    protected LiveStartedNotification() { }

    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }
}
