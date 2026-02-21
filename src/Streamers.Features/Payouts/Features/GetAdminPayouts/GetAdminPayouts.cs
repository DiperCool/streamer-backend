using GreenDonut.Data;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.SystemRoles.Services;
using Streamers.Features.Payouts.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Streamers.Features.Payouts.Features.GetAdminPayouts;

public record GetAdminPayouts(
    QueryContext<PayoutDto> Query,
    PagingArguments PagingArguments,
    DateTime? FromDate,
    DateTime? ToDate
) : IRequest<Page<PayoutDto>>;

public class GetAdminPayoutsHandler(
    StreamerDbContext context,
    ICurrentUser currentUser,
    ISystemRoleService systemRoleService
) : IRequestHandler<GetAdminPayouts, Page<PayoutDto>>
{
    public async Task<Page<PayoutDto>> Handle(
        GetAdminPayouts request,
        CancellationToken cancellationToken
    )
    {
        if (!await systemRoleService.HasAdministratorRole(currentUser.UserId))
        {
            throw new UnauthorizedAccessException();
        }

        var query = context.Payouts.AsQueryable();

        if (request.FromDate is not null)
        {
            query = query.Where(x => x.CreatedAt >= request.FromDate);
        }

        if (request.ToDate is not null)
        {
            query = query.Where(x => x.CreatedAt <= request.ToDate);
        }

        var dtoQuery = query.Select(p => new PayoutDto
        {
            Id = p.Id,
            StreamerId = p.StreamerId,
            StripePayoutId = p.StripePayoutId,
            Amount = p.Amount,
            Currency = p.Currency,
            Status = p.Status,
            ArrivalDate = p.ArrivalDate,
            FailureMessage = p.FailureMessage,
            CreatedAt = p.CreatedAt,
        });

        Page<PayoutDto> result = await dtoQuery
            .With(request.Query)
            .ToPageAsync(request.PagingArguments, cancellationToken: cancellationToken);

        return result;
    }
}
