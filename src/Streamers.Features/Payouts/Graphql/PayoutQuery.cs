using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Payouts.Dtos;
using Streamers.Features.Payouts.Features.GetPayouts;

namespace Streamers.Features.Payouts.Graphql;

[QueryType]
public static partial class PayoutQuery
{
    [UsePaging(MaxPageSize = 25)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<PayoutDto>> GetPayoutsAsync(
        string streamerId,
        [Service] IMediator mediator,
        QueryContext<PayoutDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetPayouts(streamerId, rcontext, offsetPagingArguments),
            cancellationToken
        );
        return result.ToConnection();
    }
}
