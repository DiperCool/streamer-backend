using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Tags.Dto;

namespace Streamers.Features.Tags.Features.GetTagsByStreamIds;

public record GetTagsByStreamIds(IReadOnlyList<Guid> Ids) : IRequest<ILookup<Guid, TagDto>>;

public class GetTagsByStreamIdsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetTagsByStreamIds, ILookup<Guid, TagDto>>
{
    public async Task<ILookup<Guid, TagDto>> Handle(
        GetTagsByStreamIds request,
        CancellationToken cancellationToken
    )
    {
        var lookup = await streamerDbContext
            .Streams.Where(si => request.Ids.Contains(si.Id))
            .SelectMany(si =>
                si.Tags.Select(t => new
                {
                    StreamInfoId = si.Id,
                    Tag = new TagDto { Id = t.Id, Title = t.Title },
                })
            )
            .ToListAsync(cancellationToken);

        return lookup.ToLookup(x => x.StreamInfoId, x => x.Tag);
    }
}
