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
        const int only = 10;
        var search = request.Search;
        var categories = streamerDbContext
            .Categories.AsNoTracking()
            .Where(x => EF.Functions.ILike(x.Title, $"%{search}%"))
            .Select(x => new
            {
                Title = x.Title,
                Slug = x.Slug,
                Image = (string?)x.Image,
                ResultType = SearchResultType.Category,
            });

        var streamers = streamerDbContext
            .Streamers.AsNoTracking()
            .Where(x => x.FinishedAuth && EF.Functions.ILike(x.UserName!, $"%{search}%"))
            .Select(x => new
            {
                Title = x.UserName!,
                Slug = x.UserName!,
                Image = (string?)x.Avatar,
                ResultType = SearchResultType.Streamer,
            });

        var union = categories.Union(streamers);

        var results = await union
            .OrderBy(x => x.Title)
            .Take(only)
            .Select(x => new SearchResult(x.Title, x.Slug, x.Image, x.ResultType))
            .ToListAsync(cancellationToken);

        return results;
    }
}
