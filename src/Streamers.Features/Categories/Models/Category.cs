using Shared.Abstractions.Domain;

namespace Streamers.Features.Categories.Models;

public class Category : Entity
{
    protected Category() { }

    public Category(string title, string slug, string image)
    {
        Title = title;
        Slug = slug;
        Image = image;
    }

    public string Title { get; set; }
    public string Slug { get; set; }
    public string Image { get; set; }
}
