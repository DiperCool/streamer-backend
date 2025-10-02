using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Followers.Models;

public class Follower : Entity
{
    protected Follower() { }

    public Follower(Streamer follower, Streamer streamer, DateTime followedAt)
    {
        FollowerStreamer = follower;
        Streamer = streamer;
        FollowedAt = followedAt;
    }

    public string FollowerStreamerId { get; set; }
    public Streamer FollowerStreamer { get; set; }

    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }

    public DateTime FollowedAt { get; set; }
}
