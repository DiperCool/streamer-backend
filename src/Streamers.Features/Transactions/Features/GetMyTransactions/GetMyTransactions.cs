using GreenDonut.Data;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Transactions.Dtos;

namespace Streamers.Features.Transactions.Features.GetMyTransactions;

public record GetMyTransactions(
    QueryContext<UserTransactionDto> Query,
    PagingArguments PagingArguments
) : IRequest<Page<UserTransactionDto>>;

public class GetMyTransactionsHandler(StreamerDbContext context, ICurrentUser currentUser)
    : IRequestHandler<GetMyTransactions, Page<UserTransactionDto>>
{
    public async Task<Page<UserTransactionDto>> Handle(
        GetMyTransactions request,
        CancellationToken cancellationToken
    )
    {
        var query = context.Transactions.Where(s => s.UserId == currentUser.UserId);

        var dtoQuery = query.Select(s => new UserTransactionDto
        {
            Id = s.Id,
            StreamerId = s.StreamerId,
            CreatedAt = s.CreatedAt,
            TransactionType = s.TransactionType,
            GrossAmount = s.GrossAmount,
            Status = s.Status,
            StripeInvoiceUrl = s.StripeInvoiceUrl,
        });

        Page<UserTransactionDto> result = await dtoQuery
            .With(request.Query)
            .ToPageAsync(request.PagingArguments, cancellationToken: cancellationToken);

        return result;
    }
}
