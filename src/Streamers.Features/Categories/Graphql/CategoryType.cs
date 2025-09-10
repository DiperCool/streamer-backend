using HotChocolate;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Categories.Models;

namespace Streamers.Features.Categories.Graphql;

[ObjectType<CategoryDto>]
public static partial class CategoryType
{
    public static async Task<long> GetWatchers(
        [Parent(nameof(CategoryDto.Id))] CategoryDto category,
        CategoryWatchersByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        return await dataLoader.LoadAsync(category.Id, cancellationToken);
    }
}
