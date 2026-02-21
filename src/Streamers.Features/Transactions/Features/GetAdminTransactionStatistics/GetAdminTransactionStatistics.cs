using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Payouts.Enums;
using Streamers.Features.Transactions.Models;
using Streamers.Features.Shared.Persistance;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.SystemRoles.Services;

namespace Streamers.Features.Transactions.Features.GetAdminTransactionStatistics;

public record GetAdminTransactionStatistics(DateTime FromDate, DateTime ToDate) : IRequest<GetAdminTransactionStatisticsResponse>;

public record GetAdminTransactionStatisticsResponse
{
    public decimal TotalGrossVolume { get; set; }
    public decimal TotalPlatformNet { get; set; }
    public int TransactionsCount { get; set; }
    public decimal TotalPaidOut { get; set; }
    public int SuccessfulPayoutsCount { get; set; }
}

public class GetAdminTransactionStatisticsHandler(
    StreamerDbContext dbContext,
    ICurrentUser currentUser,
    ISystemRoleService systemRoleService
) : IRequestHandler<GetAdminTransactionStatistics, GetAdminTransactionStatisticsResponse>
{
    public async Task<GetAdminTransactionStatisticsResponse> Handle(GetAdminTransactionStatistics request, CancellationToken cancellationToken)
    {
        if (!await systemRoleService.HasAdministratorRole(currentUser.UserId))
        {
            throw new UnauthorizedAccessException();
        }

        var transactionStats = await dbContext.Transactions
            .AsNoTracking()
            .Where(t => t.CreatedAt >= request.FromDate && t.CreatedAt <= request.ToDate && t.Status == TransactionStatus.Succeeded)
            .GroupBy(t => 1)
            .Select(g => new
            {
                TotalGrossVolume = g.Sum(t => t.GrossAmount),
                TotalPlatformNet = g.Sum(t => t.PlatformFee),
                TransactionsCount = g.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        var payoutStats = await dbContext.Payouts
            .AsNoTracking()
            .Where(p => p.CreatedAt >= request.FromDate && p.CreatedAt <= request.ToDate && p.Status == PayoutStatus.Paid)
            .GroupBy(p => 1)
            .Select(g => new
            {
                TotalPaidOut = g.Sum(p => p.Amount),
                SuccessfulPayoutsCount = g.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);


        return new GetAdminTransactionStatisticsResponse
        {
            TotalGrossVolume = transactionStats?.TotalGrossVolume ?? 0,
            TotalPlatformNet = transactionStats?.TotalPlatformNet ?? 0,
            TransactionsCount = transactionStats?.TransactionsCount ?? 0,
            TotalPaidOut = payoutStats?.TotalPaidOut ?? 0,
            SuccessfulPayoutsCount = payoutStats?.SuccessfulPayoutsCount ?? 0
        };
    }
}
