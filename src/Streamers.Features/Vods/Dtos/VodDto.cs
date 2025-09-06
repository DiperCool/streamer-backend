namespace Streamers.Features.Vods.Dtos;

public class VodDto
{
    public required Guid Id { get; set; }
    public required string? Source { get; set; }
    public required string? Title { get; set; }
    public required string? Preview { get; set; }
    public required string? Description { get; set; }
    public required long Views { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required long Duration { get; set; }
    public required string StreamerId { get; set; }
    public required Guid? CategoryId { get; set; }
    public required string Language { get; set; }
}
