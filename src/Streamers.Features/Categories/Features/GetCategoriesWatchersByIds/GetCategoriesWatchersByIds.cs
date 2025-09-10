using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Categories.Features.GetCategoriesWatchersByIds;

public record GetCategoriesWatchersByIds(IReadOnlyList<Guid> Ids)
    : IRequest<IDictionary<Guid, long>>;

public class GetCategoriesWatchersByIdsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetCategoriesWatchersByIds, IDictionary<Guid, long>>
{
    public async Task<IDictionary<Guid, long>> Handle(
        GetCategoriesWatchersByIds request,
        CancellationToken cancellationToken
    )
    {
        var streams = await streamerDbContext
            .Streams.Where(x => request.Ids.Contains(x.CategoryId!.Value) && x.Active)
            .GroupBy(x => x.CategoryId!.Value)
            .ToDictionaryAsync(
                x => x.Key,
                x => x.Sum(stream => stream.CurrentViewers),
                cancellationToken: cancellationToken
            );
        return streams;
    }
}
