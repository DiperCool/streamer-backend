using HotChocolate;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Categories.Graphql;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;
using Streamers.Features.Streams.Dtos;
using Streamers.Features.Tags.Dto;
using Streamers.Features.Tags.Graphql;

namespace Streamers.Features.Streams.GraphQl;

[ObjectType<StreamDto>]
public static partial class StreamType
{
    public static async Task<StreamerDto?> GetStreamer(
        [Parent(nameof(StreamDto.StreamerId))] StreamDto stream,
        IStreamersByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        var streamer = await dataLoader.LoadAsync(stream.StreamerId, cancellationToken);
        return streamer;
    }

    public static async Task<StreamSourceDto[]> GetSources(
        [Parent] StreamDto stream,
        ISourcesByStreamIdsDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        var streamers = await dataLoader.LoadAsync(stream.Id, cancellationToken);
        return streamers ?? [];
    }

    public static async Task<TagDto[]> GetTags(
        [Parent(nameof(StreamDto.Id))] StreamDto streamDto,
        TagsByStreamIdsDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        return await dataLoader.LoadAsync(streamDto.Id, cancellationToken) ?? [];
    }

    public static async Task<CategoryDto?> GetCategory(
        [Parent(nameof(StreamDto.CategoryId))] StreamDto streamDto,
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
}
