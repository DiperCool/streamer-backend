using Streamers.Features.Streams.Enums;

namespace Streamers.Features.Streams.Dtos;

public class StreamSourceDto
{
    public required StreamSourceType SourceType { get; set; }
    public required string Url { get; set; }
    public required Guid StreamId { get; set; }
}
