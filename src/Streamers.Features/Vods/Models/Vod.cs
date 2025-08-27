using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Vods.Enums;
using Stream = Streamers.Features.Streams.Models.Stream;

namespace Streamers.Features.Vods.Models;

public record VodCreated(Vod Vod) : IDomainEvent;

public class Vod : Entity
{
    protected Vod(string streamHls)
    {
        StreamHls = streamHls;
    }

    public Vod(Guid id, Streamer streamer, DateTime createdAt, string streamHls)
    {
        Id = id;
        Status = VodStatus.Progress;
        Streamer = streamer;
        CreatedAt = createdAt;
        StreamHls = streamHls;
        Raise(new VodCreated(this));
    }

    public VodStatus Status { get; private set; }
    public string StreamHls { get; set; }
    public string? Source { get; private set; }
    public string? Title { get; set; }
    public string? Preview { get; private set; }
    public string? Description { get; set; }
    public long Views { get; set; }

    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }

    public DateTime CreatedAt { get; set; }
    public long Duration { get; set; }

    public void Finish(
        string source,
        string preview,
        long duration,
        string title,
        string description
    )
    {
        Source = source;
        Preview = preview;
        Title = title;
        Status = VodStatus.Finished;
        Description = description;
        Duration = duration;
    }
}
