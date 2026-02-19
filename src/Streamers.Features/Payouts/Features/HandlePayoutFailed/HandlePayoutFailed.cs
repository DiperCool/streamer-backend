using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Payouts.Features.HandlePayoutFailed;

public record HandlePayoutFailed(string StripePayoutId, string FailureMessage)
    : IRequest<HandlePayoutFailedResponse>;

public record HandlePayoutFailedResponse(bool IsSuccess, string? ErrorMessage = null);

public class HandlePayoutFailedHandler(StreamerDbContext dbContext)
    : IRequestHandler<HandlePayoutFailed, HandlePayoutFailedResponse>
{
    public async Task<HandlePayoutFailedResponse> Handle(
        HandlePayoutFailed request,
        CancellationToken cancellationToken
    )
    {
        var payout = await dbContext.Payouts.FirstOrDefaultAsync(
            x => x.StripePayoutId == request.StripePayoutId,
            cancellationToken
        );

        if (payout is null)
        {
            return new HandlePayoutFailedResponse(false, "Payout not found.");
        }

        payout.MakeFailed(request.FailureMessage);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new HandlePayoutFailedResponse(true);
    }
}
