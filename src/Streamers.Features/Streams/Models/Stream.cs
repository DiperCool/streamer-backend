using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Streams.Models;

public record StreamUpdated(Stream Stream) : IDomainEvent;

public class Stream : Entity
{
    protected Stream() { }

    public Stream(
        Streamer streamer,
        string streamId,
        string title,
        DateTime started,
        List<StreamSource> streamSources
    )
    {
        Streamer = streamer;
        StreamerId = streamer.Id;
        StreamId = streamId;
        Title = title;
        Started = started;
        StreamSources = streamSources;
        Active = true;
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

    public void SetActive(bool active)
    {
        Active = active;
        Raise(new StreamUpdated(this));
    }

    public void SetCurrentViewers(long viewers)
    {
        CurrentViewers = viewers;
        Raise(new StreamUpdated(this));
    }
}
