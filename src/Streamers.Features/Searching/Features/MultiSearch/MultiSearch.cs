using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Searching.Enums;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Searching.Features.MultiSearch;

public record SearchResult(string Title, string Slug, string? Image, SearchResultType ResultType);

public record MultiSearch(string Search) : IRequest<List<SearchResult>>;

public class MultiSearchHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<MultiSearch, List<SearchResult>>
{
    public async Task<List<SearchResult>> Handle(
        MultiSearch request,
        CancellationToken cancellationToken
    )
    {
        var search = request.Search;

        var categories = streamerDbContext
            .Categories.AsNoTracking()
            .Where(x => EF.Functions.ILike(x.Title, $"%{search}%"))
            .Select(x => new SearchResult(x.Title, x.Slug, x.Image, SearchResultType.Category));

        var streamers = streamerDbContext
            .Streamers.AsNoTracking()
            .Where(x => x.FinishedAuth && EF.Functions.ILike(x.UserName!, $"%{search}%"))
            .Select(x => new SearchResult(
                x.UserName!,
                x.UserName!,
                x.Avatar,
                SearchResultType.Streamer
            ));

        var results = await categories
            .Union(streamers)
            .OrderBy(x => x.Title)
            .Take(10)
            .ToListAsync(cancellationToken);
        return results;
    }
}
