using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.ModerationActivities.Dtos;
using Streamers.Features.ModerationActivities.Features.GetModerationActivitiesByStreamer;

namespace Streamers.Features.ModerationActivities.Graphql;

[QueryType]
public static class ModerationActivitiesQuery
{
    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<ModeratorActionDto>> GetModerationActivities(
        string streamerId,
        [Service] IMediator mediator,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetModerationActivitiesByStreamer(streamerId, offsetPagingArguments),
            cancellationToken
        );
        return result.ToConnection();
    }
}
