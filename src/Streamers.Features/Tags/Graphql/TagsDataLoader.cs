using GreenDonut;
using HotChocolate;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Tags.Dto;
using Streamers.Features.Tags.Features.GetTagsByStreamIds;
using Streamers.Features.Tags.Features.GetTagsByStreamInfoIds;
using Streamers.Features.Tags.Features.GetTagsByVodIds;
using Streamers.Features.Vods.Dtos;

namespace Streamers.Features.Tags.Graphql;

public static partial class TagsDataLoader
{
    [DataLoader]
    public static async Task<ILookup<Guid, TagDto>> GetTagsByStreamInfoIds(
        IReadOnlyList<Guid> ids,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new GetTagsByStreamInfoIds(ids), cancellationToken);
        return response;
    }

    [DataLoader]
    public static async Task<ILookup<Guid, TagDto>> GetTagsByStreamIds(
        IReadOnlyList<Guid> ids,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new GetTagsByStreamIds(ids), cancellationToken);
        return response;
    }

    [DataLoader]
    public static async Task<ILookup<Guid, TagDto>> GetTagsByVodsIds(
        IReadOnlyList<Guid> ids,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new GetTagsByVodIds(ids), cancellationToken);
        return response;
    }
}
