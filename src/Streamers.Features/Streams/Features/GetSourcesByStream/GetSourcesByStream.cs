using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streams.Dtos;

namespace Streamers.Features.Streams.Features.GetSourcesByStream;

public record GetSourcesByStream(IReadOnlyList<Guid> StreamsIds)
    : IRequest<ILookup<Guid, StreamSourceDto>>;

public class GetSourcesByStreamHandler(StreamerDbContext context)
    : IRequestHandler<GetSourcesByStream, ILookup<Guid, StreamSourceDto>>
{
    public async Task<ILookup<Guid, StreamSourceDto>> Handle(
        GetSourcesByStream request,
        CancellationToken cancellationToken
    )
    {
        List<StreamSourceDto> sources = await context
            .StreamSources.AsNoTracking()
            .Where(x => request.StreamsIds.Contains(x.StreamId))
            .Select(x => new StreamSourceDto
            {
                SourceType = x.SourceType,
                Url = x.Url,
                StreamId = x.StreamId,
            })
            .ToListAsync(cancellationToken: cancellationToken);
        return sources.ToLookup(x => x.StreamId);
    }
}
