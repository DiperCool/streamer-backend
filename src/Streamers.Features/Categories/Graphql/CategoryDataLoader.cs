using GreenDonut;
using HotChocolate;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Categories.Features.GetCategoriesByIds;

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
}
