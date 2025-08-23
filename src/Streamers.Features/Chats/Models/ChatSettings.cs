using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Chats.Models;

public class ChatSettings : Entity
{
    public int? SlowMode { get; set; }
    public bool FollowersOnly { get; set; }
    public bool SubscribersOnly { get; set; }
    public List<string> BannedWords { get; set; } = new List<string>();

    public Chat Chat { get; set; }
    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }

    public void Update(
        int? slowMode,
        bool followersOnly,
        bool subscribersOnly,
        List<string> bannedWords
    )
    {
        SlowMode = slowMode;
        FollowersOnly = followersOnly;
        SubscribersOnly = subscribersOnly;
        BannedWords = bannedWords;
        Raise(new ChatUpdated(Chat));
    }
}
