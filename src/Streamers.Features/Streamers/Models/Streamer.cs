using Shared.Abstractions.Domain;
using Streamers.Features.Profiles.Models;
using Streamers.Features.Settings.Models;

namespace Streamers.Features.Streamers.Models;

public class Streamer : Entity<string>
{
    public string UserName { get; set; }
    public string? Avatar { get; set; }
    public string Email { get; set; }
    public long Followers { get; set; }
    public Setting Setting { get; set; }
    public Profile Profile { get; set; }
    public DateTime CreatedAt { get; set; }

    private Streamer() { }

    public Streamer(
        string id,
        string userName,
        string email,
        Profile profile,
        Setting setting,
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
    }
}
