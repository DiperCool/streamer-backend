using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Categories.Features.GetCategories;
using Streamers.Features.Categories.Features.GetCategory;

namespace Streamers.Features.Categories.Graphql;

[QueryType]
public static partial class CategoryQueries
{
    public static async Task<CategoryDto> GetCategory(Guid id, [Service] IMediator mediator)
    {
        var response = await mediator.Send(new GetCategory(id));
        return response;
    }

    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<CategoryDto>> GetCategories(
        string? search,
        [Service] IMediator mediator,
        QueryContext<CategoryDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetCategories(search, offsetPagingArguments, rcontext),
            cancellationToken
        );
        return result.ToConnection();
    }
}
