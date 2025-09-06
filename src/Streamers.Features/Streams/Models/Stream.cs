using Shared.Abstractions.Domain;
using Streamers.Features.Categories.Models;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Tags.Models;

namespace Streamers.Features.Streams.Models;

public record StreamUpdated(Stream Stream) : IDomainEvent;

public record StreamCreated(Stream Stream) : IDomainEvent;

public class Stream : Entity
{
    protected Stream() { }

    public Stream(
        Streamer streamer,
        string streamId,
        string title,
        DateTime started,
        List<StreamSource> streamSources,
        string language,
        List<Tag> tags
    )
    {
        Streamer = streamer;
        StreamerId = streamer.Id;
        StreamId = streamId;
        Title = title;
        Started = started;
        StreamSources = streamSources;
        Language = language;
        Tags = tags;
        Active = true;
        Raise(new StreamCreated(this));
        SetActive(Active);
    }

    public string StreamId { get; set; }
    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }
    public bool Active { get; private set; }
    public string Title { get; set; }
    public long CurrentViewers { get; private set; }
    public DateTime Started { get; set; }
    public List<StreamSource> StreamSources { get; set; }
    public List<Tag> Tags { get; set; }
    public string Language { get; set; }
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    public double? Duration { get; set; }

    public void SetActive(bool active)
    {
        Active = active;
        if (!active)
        {
            Duration = DateTime.UtcNow.Subtract(Started).TotalSeconds;
        }
        Raise(new StreamUpdated(this));
    }

    public void SetCurrentViewers(long viewers)
    {
        CurrentViewers = viewers;
        Raise(new StreamUpdated(this));
    }

    public void Update(string title, string language, List<Tag> tags, Category? category)
    {
        CategoryId = category?.Id;
        Category = category;
        Title = title;
        Language = language;
        Tags.Clear();
        Tags.AddRange(tags);
        Raise(new StreamUpdated(this));
    }
}
