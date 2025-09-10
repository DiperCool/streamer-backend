using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Categories.Features.GetCategories;
using Streamers.Features.Categories.Features.GetCategory;
using Streamers.Features.Categories.Features.GetCategoryBySlug;
using Streamers.Features.Categories.Features.GetTopCategories;

namespace Streamers.Features.Categories.Graphql;

[QueryType]
public static partial class CategoryQueries
{
    public static async Task<CategoryDto> GetCategory(Guid id, [Service] IMediator mediator)
    {
        var response = await mediator.Send(new GetCategory(id));
        return response;
    }

    public static async Task<CategoryDto> GetCategoryBySlug(
        string slug,
        [Service] IMediator mediator
    )
    {
        var response = await mediator.Send(new GetCategoryBySlug(slug));
        return response;
    }

    public static async Task<List<CategoryDto>> GetTopCategories([Service] IMediator mediator)
    {
        var response = await mediator.Send(new GetTopCategories());
        return response;
    }

    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<CategoryDto>> GetCategories(
        string? search,
        Guid? tag,
        [Service] IMediator mediator,
        QueryContext<CategoryDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetCategories(search, tag, offsetPagingArguments, rcontext),
            cancellationToken
        );
        return result.ToConnection();
    }
}
