using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streams.Dtos;

namespace Streamers.Features.Streams.Features.GetStreamsByIds;

public record GetStreamsByIdsResponse(IDictionary<Guid, StreamDto> Streams);

public record GetStreamsByIds(IReadOnlyList<Guid> Ids) : IRequest<GetStreamsByIdsResponse>;

public class GetStreamersByIdsHandler(StreamerDbContext context)
    : IRequestHandler<GetStreamsByIds, GetStreamsByIdsResponse>
{
    public async Task<GetStreamsByIdsResponse> Handle(
        GetStreamsByIds request,
        CancellationToken cancellationToken
    )
    {
        var streamers = await context
            .Streams.Where(s => request.Ids.Contains(s.Id))
            .Select(s => new StreamDto
            {
                Id = s.Id,
                StreamerId = s.StreamerId,
                Active = s.Active,
                Title = s.Title,
                CurrentViewers = s.CurrentViewers,
                Language = s.Language,
                CategoryId = s.CategoryId,
                Started = s.Started,
            })
            .ToListAsync(cancellationToken);

        var dict = streamers.ToDictionary(s => s.Id, s => s);

        return new GetStreamsByIdsResponse(dict);
    }
}
