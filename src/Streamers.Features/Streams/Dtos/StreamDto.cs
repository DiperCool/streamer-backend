namespace Streamers.Features.Streams.Dtos;

public class StreamDto
{
    public required Guid Id { get; set; }
    public required string StreamerId { get; set; }
    public required bool Active { get; set; }
    public required string Title { get; set; }
    public required long CurrentViewers { get; set; }
    public required Guid? CategoryId { get; set; }
    public required string Language { get; set; }
    public required DateTime Started { get; set; }
    public required string? Preview { get; set; }
}
