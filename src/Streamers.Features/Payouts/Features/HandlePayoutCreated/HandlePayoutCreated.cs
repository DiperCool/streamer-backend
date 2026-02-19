using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Payout = Streamers.Features.Payouts.Models.Payout;

namespace Streamers.Features.Payouts.Features.HandlePayoutCreated;

public record HandlePayoutCreated(
    string StripePayoutId,
    string StripeAccountId,
    decimal Amount,
    string Currency,
    DateTime ArrivalDate,
    string Status,
    decimal ApplicationFee
) : IRequest<HandlePayoutCreatedResponse>;

public record HandlePayoutCreatedResponse(
    bool IsSuccess,
    Guid? PayoutId = null,
    string? ErrorMessage = null
);

public class HandlePayoutCreatedHandler(StreamerDbContext dbContext)
    : IRequestHandler<HandlePayoutCreated, HandlePayoutCreatedResponse>
{
    public async Task<HandlePayoutCreatedResponse> Handle(
        HandlePayoutCreated request,
        CancellationToken cancellationToken
    )
    {
        var existingPayout = await dbContext.Payouts.FirstOrDefaultAsync(
            x => x.StripePayoutId == request.StripePayoutId,
            cancellationToken
        );

        if (existingPayout != null)
        {
            return new HandlePayoutCreatedResponse(
                true,
                existingPayout.Id,
                "Payout already exists."
            );
        }

        var partner = await dbContext.Partners.FirstOrDefaultAsync(
            p => p.StripeAccountId == request.StripeAccountId,
            cancellationToken
        );

        if (partner is null)
        {
            return new HandlePayoutCreatedResponse(
                false,
                null,
                $"Partner not found for Stripe Account ID: {request.StripeAccountId}."
            );
        }

        var payout = new Payout(
            Guid.NewGuid(),
            partner.StreamerId,
            request.StripePayoutId,
            request.Amount,
            request.Currency,
            request.Status,
            request.ArrivalDate,
            DateTime.UtcNow,
            request.ApplicationFee
        );

        dbContext.Payouts.Add(payout);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new HandlePayoutCreatedResponse(true, payout.Id);
    }
}
