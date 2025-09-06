using Shared.Abstractions.Domain;
using Streamers.Features.Categories.Models;
using Streamers.Features.StreamInfos.Models;
using Streamers.Features.Streams.Models;
using Streamers.Features.Vods.Models;
using Stream = Streamers.Features.Streams.Models.Stream;

namespace Streamers.Features.Tags.Models;

public class Tag : Entity<Guid>
{
    public Tag(Guid id, string title)
    {
        Id = id;
        Title = title;
    }

    public string Title { get; set; }
    public List<Stream> Streams { get; set; } = new List<Stream>();
    public List<StreamInfo> StreamInfos { get; set; } = new List<StreamInfo>();
    public List<Vod> Vods { get; set; } = new List<Vod>();
}
