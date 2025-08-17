using Shared.Abstractions.Domain;
using Streamers.Features.Streams.Enums;

namespace Streamers.Features.Streams.Models;

public class StreamSource : Entity
{
    public StreamSourceType SourceType { get; set; }
    public string Url { get; set; }
    public Guid StreamId { get; set; }
    public Stream Stream { get; set; }
}
