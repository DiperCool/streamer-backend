using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Categories.Graphql;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;
using Streamers.Features.Tags.Dto;
using Streamers.Features.Tags.Graphql;
using Streamers.Features.Vods.Dtos;

namespace Streamers.Features.Vods.GraphQl;

[ObjectType<VodDto>]
public static partial class VodType
{
    public static async Task<CategoryDto?> GetCategory(
        [Parent(nameof(VodDto.CategoryId))] VodDto streamDto,
        CategoryByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        if (streamDto.CategoryId == null)
        {
            return null;
        }
        return await dataLoader.LoadAsync(streamDto.CategoryId.Value, cancellationToken);
    }

    public static async Task<StreamerDto?> GetStreamerAsync(
        [Parent(nameof(VodDto.StreamerId))] VodDto vod,
        IStreamersByIdDataLoader loader,
        CancellationToken cancellationToken
    )
    {
        var streamer = await loader.LoadAsync(vod.StreamerId, cancellationToken);
        return streamer;
    }

    public static async Task<TagDto[]> GetTags(
        [Parent(nameof(VodDto.Id))] VodDto streamDto,
        TagsByVodsIdsDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        return await dataLoader.LoadAsync(streamDto.Id, cancellationToken) ?? [];
    }
}
