using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Categories.Graphql;
using Streamers.Features.StreamInfos.Dtos;
using Streamers.Features.Tags.Dto;
using Streamers.Features.Tags.Graphql;

namespace Streamers.Features.StreamInfos.Graphql;

[ObjectType<StreamInfoDto>]
public static partial class StreamInfoType
{
    public static async Task<CategoryDto?> GetCategory(
        [Parent(nameof(StreamInfoDto.CategoryId))] StreamInfoDto streamInfo,
        CategoryByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        if (streamInfo.CategoryId == null)
        {
            return null;
        }
        return await dataLoader.LoadAsync(streamInfo.CategoryId.Value, cancellationToken);
    }

    public static async Task<TagDto[]> GetTags(
        [Parent(nameof(StreamInfoDto.Id))] StreamInfoDto streamInfo,
        TagsByStreamInfoIdsDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        return await dataLoader.LoadAsync(streamInfo.Id, cancellationToken) ?? [];
    }
}
