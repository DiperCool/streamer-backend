using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Searching.Features.MultiSearch;

namespace Streamers.Features.Searching.Graphql;

[QueryType]
public static partial class SearchQuery
{
    [AllowAnonymous]
    public static async Task<List<SearchResult>> Search(string search, [Service] IMediator mediator)
    {
        return await mediator.Send(new MultiSearch(search));
    }
}
