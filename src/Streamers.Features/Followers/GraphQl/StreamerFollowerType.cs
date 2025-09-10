using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Categories.Graphql;
using Streamers.Features.Followers.Dtos;
using Streamers.Features.Streams.Dtos;
using Streamers.Features.Streams.GraphQl;

namespace Streamers.Features.Followers.GraphQl;

[ObjectType<StreamerFollowerDto>]
public static partial class StreamerFollowerType
{
    public static async Task<StreamDto?> GetCurrentStream(
        [Parent(nameof(StreamerFollowerDto.CurrentStreamId))] StreamerFollowerDto streamDto,
        StreamByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        if (streamDto.CurrentStreamId == null)
        {
            return null;
        }
        return await dataLoader.LoadAsync(streamDto.CurrentStreamId.Value, cancellationToken);
    }
}
