using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Profiles.Models;

public class Profile : Entity
{
    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }
    public string? Bio { get; set; }
    public string? ChannelBanner { get; set; }
    public string? OfflineStreamBanner { get; set; }
    public string? Instagram { get; set; }
    public string? Youtube { get; set; }
    public string? Discord { get; set; }
}
