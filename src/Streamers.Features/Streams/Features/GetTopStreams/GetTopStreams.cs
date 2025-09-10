using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streams.Dtos;

namespace Streamers.Features.Streams.Features.GetTopStreams;

public record GetTopStreams() : IRequest<List<StreamDto>>;

public class GetTopStreamsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetTopStreams, List<StreamDto>>
{
    public async Task<List<StreamDto>> Handle(
        GetTopStreams request,
        CancellationToken cancellationToken
    )
    {
        const int only = 4;
        List<StreamDto> streams = await streamerDbContext
            .Streams.OrderByDescending(x => x.CurrentViewers)
            .Select(x => new StreamDto
            {
                Id = x.Id,
                StreamerId = x.StreamerId,
                Active = x.Active,
                Title = x.Title,
                CurrentViewers = x.CurrentViewers,
                CategoryId = x.CategoryId,
                Language = x.Language,
                Started = x.Started,
                Preview = x.Preview,
            })
            .Where(x => x.Active)
            .Take(only)
            .ToListAsync(cancellationToken: cancellationToken);
        return streams;
    }
}
