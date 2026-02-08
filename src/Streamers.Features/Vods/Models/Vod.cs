using Shared.Abstractions.Domain;
using Streamers.Features.Categories.Models;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Tags.Models;
using Streamers.Features.Vods.Enums;

namespace Streamers.Features.Vods.Models;

public record VodCreated(Vod Vod) : IDomainEvent;

public class Vod : Entity
{
    protected Vod() { }

    public Vod(Guid id, Streamer streamer, DateTime createdAt, string streamHls, string language)
    {
        Id = id;
        Status = VodStatus.Progress;
        Streamer = streamer;
        CreatedAt = createdAt;
        StreamHls = streamHls;
        Language = language;
        Type = VodType.Public;
        Raise(new VodCreated(this));
    }

    public VodType Type { get; set; }
    public VodStatus Status { get; private set; }
    public string StreamHls { get; set; }
    public string? Source { get; private set; }
    public string? Title { get; set; }
    public string? Preview { get; private set; }
    public string? Description { get; set; }
    public long Views { get; set; }
    public List<Tag> Tags { get; set; } = new List<Tag>();

    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }

    public DateTime CreatedAt { get; set; }
    public long Duration { get; set; }
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    public string Language { get; set; }

    public void Finish(
        string source,
        string preview,
        long duration,
        string title,
        string description,
        Category? category,
        List<Tag> tags,
        string language
    )
    {
        Source = source;
        Preview = preview;
        Title = title;
        Status = VodStatus.Finished;
        Description = description;
        Duration = duration;
        CategoryId = category?.Id;
        Category = category;
        Tags.AddRange(tags);
        Language = language;
    }

    public void Update(
        VodType type,
        string title,
        string description,
        Category? category,
        List<Tag> tags,
        string language
    )
    {
        Type = type;
        Title = title;
        Description = description;
        CategoryId = category?.Id;
        Category = category;
        Tags.Clear();
        Tags.AddRange(tags);
        Language = language;
    }
}
