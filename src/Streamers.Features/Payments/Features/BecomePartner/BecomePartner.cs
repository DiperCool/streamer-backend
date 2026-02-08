using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Payments.Features.BecomePartner;

public record BecomePartner() : IRequest<BecomePartnerResponse>;

public record BecomePartnerResponse(string Id);

public class BecomePartnerHandler(
    StreamerDbContext context,
    ICurrentUser currentUser,
    IStripeService stripeService
) : IRequestHandler<BecomePartner, BecomePartnerResponse>
{
    public async Task<BecomePartnerResponse> Handle(
        BecomePartner request,
        CancellationToken cancellationToken
    )
    {
        var streamerId = currentUser.UserId;

        var partner = await context
            .Partners.Include(x => x.Streamer)
            .FirstOrDefaultAsync(s => s.StreamerId == streamerId, cancellationToken);

        if (partner == null)
        {
            throw new InvalidOperationException($"Streamer with ID {streamerId} not found.");
        }

        var account = await stripeService.CreateExpressAccountAsync(
            partner.Streamer.Email,
            partner.StreamerId,
            cancellationToken
        );

        partner.StartOnboarding(account.AccountId);

        await context.SaveChangesAsync(cancellationToken);

        return new BecomePartnerResponse(streamerId);
    }
}
