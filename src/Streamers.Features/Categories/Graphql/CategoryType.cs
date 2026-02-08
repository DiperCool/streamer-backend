using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Categories.Dtos;

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
