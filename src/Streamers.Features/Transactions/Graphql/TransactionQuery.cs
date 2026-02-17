using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Transactions.Dtos;
using Streamers.Features.Transactions.Features.GetMyTransactions;

namespace Streamers.Features.Transactions.Graphql;

[QueryType]
public static partial class TransactionQuery
{
    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
    [Authorize]
    public static async Task<Connection<UserTransactionDto>> GetMyTransactionsAsync(
        [Service] IMediator mediator,
        QueryContext<UserTransactionDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetMyTransactions(rcontext, offsetPagingArguments),
            cancellationToken
        );
        return result.ToConnection();
    }
}
