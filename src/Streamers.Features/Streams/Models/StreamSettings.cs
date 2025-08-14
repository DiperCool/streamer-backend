using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Streams.Models;

public class StreamSettings : Entity
{
    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }

    public Guid StreamId { get; set; }
    public string StreamKey { get; set; }
    public string StreamKeyToken { get; set; }
    public string StreamUrl { get; set; }
    public string StreamName { get; set; }
}
