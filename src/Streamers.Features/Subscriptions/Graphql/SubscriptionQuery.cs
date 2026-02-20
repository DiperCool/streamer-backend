using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Subscriptions.Dtos;
using Streamers.Features.Subscriptions.Features.GetMySubscriptions;
using Streamers.Features.Subscriptions.Features.GetSubscriptions;
using Streamers.Features.Subscriptions.Features.GetStreamerSubscriptionsStats;

namespace Streamers.Features.Subscriptions.Graphql;

[QueryType]
public static partial class SubscriptionQuery
{
    [UsePaging(MaxPageSize = 15)]
    [Authorize]
    public static async Task<Connection<SubscriptionDto>> GetMySubscriptionsAsync(
        [Service] IMediator mediator,
        QueryContext<SubscriptionDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetMySubscriptions(rcontext, offsetPagingArguments),
            cancellationToken
        );
        return result.ToConnection();
    }

    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
    [Authorize]
    public static async Task<Connection<SubscriptionDto>> GetSubscriptionsAsync(
        string streamerId,
        string? search,
        [Service] IMediator mediator,
        QueryContext<SubscriptionDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetSubscriptions(streamerId, search, rcontext, offsetPagingArguments),
            cancellationToken
        );
        return result.ToConnection();
    }

    [Authorize]
    public static async Task<GetStreamerSubscriptionsStatsResponse> GetStreamerSubscriptionsStatsAsync(
        string streamerId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        return await mediator.Send(
            new GetStreamerSubscriptionsStats(streamerId),
            cancellationToken
        );
    }
}
