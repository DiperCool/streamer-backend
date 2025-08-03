namespace Streamers.Api.Profiles.Models;

public class Profile
{
    public long Id { get; set; }
    public string? Bio { get; set; }
    public string? Avatar { get; set; }
    public string? ChannelBanner { get; set; }
    public string? OfflineStreamBanner  { get; set; }
    public string? Instagram { get; set; }
    public string? Youtube { get; set; }
    public string? Discord { get; set; }
}
