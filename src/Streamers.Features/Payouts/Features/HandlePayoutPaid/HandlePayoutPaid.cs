using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Payouts.Features.HandlePayoutPaid;

public record HandlePayoutPaid(string StripePayoutId) : IRequest<HandlePayoutPaidResponse>;

public record HandlePayoutPaidResponse(bool IsSuccess, string? ErrorMessage = null);

public class HandlePayoutPaidHandler(StreamerDbContext dbContext)
    : IRequestHandler<HandlePayoutPaid, HandlePayoutPaidResponse>
{
    public async Task<HandlePayoutPaidResponse> Handle(
        HandlePayoutPaid request,
        CancellationToken cancellationToken
    )
    {
        var payout = await dbContext.Payouts.FirstOrDefaultAsync(
            x => x.StripePayoutId == request.StripePayoutId,
            cancellationToken
        );

        if (payout is null)
        {
            return new HandlePayoutPaidResponse(false, "Payout not found.");
        }

        payout.MakePaid();

        await dbContext.SaveChangesAsync(cancellationToken);

        return new HandlePayoutPaidResponse(true);
    }
}
