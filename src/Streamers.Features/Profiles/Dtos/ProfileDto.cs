namespace Streamers.Features.Profiles.Dtos;

public class ProfileDto
{
    public required string? Bio { get; set; }
    public required string? ChannelBanner { get; set; }
    public required string? OfflineStreamBanner { get; set; }
    public required string? Instagram { get; set; }
    public required string? Youtube { get; set; }
    public required string? Discord { get; set; }
    public required string StreamerId { get; set; }
}
