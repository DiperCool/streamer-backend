using GreenDonut.Data;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Transactions.Dtos;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.SystemRoles.Services;

namespace Streamers.Features.Transactions.Features.GetAdminTransactions;

public record GetAdminTransactions(
    QueryContext<TransactionDto> Query,
    PagingArguments PagingArguments,
    DateTime? FromDate,
    DateTime? ToDate
) : IRequest<Page<TransactionDto>>;

public class GetAdminTransactionsHandler(
    StreamerDbContext context,
    ICurrentUser currentUser,
    ISystemRoleService systemRoleService
) : IRequestHandler<GetAdminTransactions, Page<TransactionDto>>
{
    public async Task<Page<TransactionDto>> Handle(
        GetAdminTransactions request,
        CancellationToken cancellationToken
    )
    {
        if (!await systemRoleService.HasAdministratorRole(currentUser.UserId))
        {
            throw new UnauthorizedAccessException();
        }

        var query = context.Transactions.AsQueryable();

        if (request.FromDate is not null)
        {
            query = query.Where(x => x.CreatedAt >= request.FromDate);
        }

        if (request.ToDate is not null)
        {
            query = query.Where(x => x.CreatedAt <= request.ToDate);
        }

        var dtoQuery = query.Select(s => new TransactionDto
        {
            Id = s.Id,
            UserId = s.UserId,
            StreamerId = s.StreamerId,
            CreatedAt = s.CreatedAt,
            TransactionType = s.TransactionType,
            GrossAmount = s.GrossAmount,
            PlatformFee = s.PlatformFee,
            StreamerNet = s.StreamerNet,
            Status = s.Status,
            StripeInvoiceUrl = s.StripeInvoiceUrl,
        });

        Page<TransactionDto> result = await dtoQuery
            .With(request.Query)
            .ToPageAsync(request.PagingArguments, cancellationToken: cancellationToken);

        return result;
    }
}
