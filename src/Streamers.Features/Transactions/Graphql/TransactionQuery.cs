using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Transactions.Dtos;
using Streamers.Features.Transactions.Features.GetMyTransactions;
using Streamers.Features.Transactions.Features.GetAdminTransactions;
using Streamers.Features.Transactions.Features.GetAdminTransactionStatistics;

namespace Streamers.Features.Transactions.Graphql;
[Authorize]
[QueryType]
public static partial class TransactionQuery
{
    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
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

    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<TransactionDto>> GetAdminTransactionsAsync(
        [Service] IMediator mediator,
        QueryContext<TransactionDto> rcontext,
        PagingArguments offsetPagingArguments,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetAdminTransactions(rcontext, offsetPagingArguments, fromDate, toDate),
            cancellationToken
        );
        return result.ToConnection();
    }

    public static async Task<GetAdminTransactionStatisticsResponse> GetAdminTransactionStatisticsAsync(
        [Service] IMediator mediator,
        DateTime fromDate,
        DateTime toDate,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetAdminTransactionStatistics(fromDate, toDate), cancellationToken);
    }
}
