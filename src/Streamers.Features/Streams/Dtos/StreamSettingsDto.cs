namespace Streamers.Features.Streams.Dtos;

public class StreamSettingsDto
{
    public required Guid Id { get; set; }
    public required string StreamKey { get; set; }
    public required string StreamUrl { get; set; }
}
