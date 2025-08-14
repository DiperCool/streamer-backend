using Shared.Abstractions.Domain;
using Streamers.Features.Profiles.Models;
using Streamers.Features.Settings.Models;
using Streamers.Features.Streams.Models;
using Stream = Streamers.Features.Streams.Models.Stream;

namespace Streamers.Features.Streamers.Models;

public record StreamerUpdated(Streamer Streamer) : IDomainEvent;

public class Streamer : Entity<string>
{
    public string UserName { get; set; }
    public bool IsLive { get; private set; }
    public string? Avatar { get; set; }
    public string Email { get; set; }
    public long Followers { get; set; }
    public Setting Setting { get; set; }
    public Profile Profile { get; set; }
    public StreamSettings StreamSettings { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<Stream> Streams { get; set; } = new List<Stream>();

    public Guid? CurrentStreamId { get; set; }
    public Stream? CurrentStream { get; set; }

    private Streamer() { }

    public Streamer(
        string id,
        string userName,
        string email,
        Profile profile,
        Setting setting,
        StreamSettings streamSettings,
        DateTime createdAt,
        string? avatar
    )
    {
        Id = id;
        UserName = userName;
        Email = email;
        Profile = profile;
        Setting = setting;
        CreatedAt = createdAt;
        Avatar = avatar;
        StreamSettings = streamSettings;
    }

    public void SetLive(bool live, Stream? currentStream)
    {
        IsLive = live;
        CurrentStreamId = currentStream?.Id;
        CurrentStream = currentStream;
        Raise(new StreamerUpdated(this));
    }
}
