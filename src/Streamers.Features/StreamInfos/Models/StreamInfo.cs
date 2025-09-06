using Shared.Abstractions.Domain;
using Streamers.Features.Categories.Models;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Tags.Models;

namespace Streamers.Features.StreamInfos.Models;

public class StreamInfo : Entity
{
    protected StreamInfo() { }

    public StreamInfo(string? title, string language)
    {
        Title = title;
        Language = language;
    }

    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    public string? Title { get; set; }
    public List<Tag> Tags { get; set; } = new List<Tag>();
    public string Language { get; set; }

    public void Update(string title, Category? category, List<Tag> tags, string language)
    {
        Language = language;
        Title = title;
        Category = category;
        Tags.Clear();
        Tags.AddRange(tags);
    }
}
