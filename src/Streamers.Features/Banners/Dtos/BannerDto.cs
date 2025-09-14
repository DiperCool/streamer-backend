namespace Streamers.Features.Banners.Dtos;

public class BannerDto
{
    public required Guid Id { get; set; }
    public required string? Title { get; set; }
    public required string? Description { get; set; }
    public required string? Image { get; set; }
    public required string? Url { get; set; }
}
