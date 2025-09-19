namespace Streamers.Features.Vods.Dtos;

public class VodSettingsDto
{
    public required Guid Id { get; set; }
    public required bool VodEnabled { get; set; }
}
