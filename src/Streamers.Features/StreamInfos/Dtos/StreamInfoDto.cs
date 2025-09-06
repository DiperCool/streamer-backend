namespace Streamers.Features.StreamInfos.Dtos;

public class StreamInfoDto
{
    public required Guid Id { get; set; }
    public required string StreamerId { get; set; }
    public required Guid? CategoryId { get; set; }
    public required string? Title { get; set; }
    public required string Language { get; set; }
}
