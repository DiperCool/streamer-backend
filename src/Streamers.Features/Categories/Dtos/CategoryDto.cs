namespace Streamers.Features.Categories.Dtos;

public class CategoryDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Image { get; set; }
}
