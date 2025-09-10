using GreenDonut;
using HotChocolate;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Categories.Features.GetCategoriesByIds;
using Streamers.Features.Categories.Features.GetCategoriesWatchersByIds;

namespace Streamers.Features.Categories.Graphql;

public static partial class CategoryDataLoader
{
    [DataLoader]
    public static async Task<IDictionary<Guid, CategoryDto>> GetCategoryByIdAsync(
        IReadOnlyList<Guid> ids,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new GetCategoriesByIds(ids), cancellationToken);
        return response.Categories;
    }

    [DataLoader]
    public static async Task<IDictionary<Guid, long>> GetCategoryWatchersByIdAsync(
        IReadOnlyList<Guid> ids,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new GetCategoriesWatchersByIds(ids), cancellationToken);
        return response;
    }
}
