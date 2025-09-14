using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Banners.Model;

public class Banner : Entity
{
    private Banner() { }

    public Banner(string? title, string? description, string? image, string? url, Streamer streamer)
    {
        Title = title;
        Description = description;
        Image = image;
        Url = url;
        Streamer = streamer;
    }

    public string StreamerId { get; private set; }
    public Streamer Streamer { get; private set; }
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public string? Image { get; private set; }
    public string? Url { get; private set; }

    public void Update(string? title, string? description, string? image, string? url)
    {
        Title = title;
        Description = description;
        Image = image;
        Url = url;
    }
}
