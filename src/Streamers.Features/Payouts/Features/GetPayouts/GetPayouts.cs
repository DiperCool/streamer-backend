using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Payouts.Dtos;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Payouts.Features.GetPayouts;

public record GetPayouts(
    string StreamerId,
    QueryContext<PayoutDto> Query,
    PagingArguments PagingArguments
) : IRequest<Page<PayoutDto>>;

public class GetPayoutsHandler(
    StreamerDbContext context,
    ICurrentUser currentUser,
    IRoleService roleService
) : IRequestHandler<GetPayouts, Page<PayoutDto>>
{
    public async Task<Page<PayoutDto>> Handle(
        GetPayouts request,
        CancellationToken cancellationToken
    )
    {
        if (!await roleService.HasRole(request.StreamerId, currentUser.UserId, Permissions.Revenue))
        {
            throw new ForbiddenException();
        }

        var query = context.Payouts.Where(p => p.StreamerId == request.StreamerId);

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
