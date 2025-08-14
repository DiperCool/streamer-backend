namespace Streamers.Features.Streams.Dtos;

public class StreamDto
{
    public required Guid Id { get; set; }
    public required string StreamerId { get; set; }
    public required bool Active { get; set; }
    public required string Title { get; set; }
    public required long CurrentViewers { get; set; }
}
