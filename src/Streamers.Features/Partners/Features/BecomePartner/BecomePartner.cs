using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Exceptions;

namespace Streamers.Features.Partners.Features.BecomePartner;

public record BecomePartner(string StreamerId) : IRequest<BecomePartnerResponse>;

public record BecomePartnerResponse(string Id);

public class BecomePartnerHandler(
    StreamerDbContext context,
    ICurrentUser currentUser,
    IStripeService stripeService,
    IRoleService roleService
) : IRequestHandler<BecomePartner, BecomePartnerResponse>
{
    public async Task<BecomePartnerResponse> Handle(
        BecomePartner request,
        CancellationToken cancellationToken
    )
    {
        var streamerId = request.StreamerId;

        if (
            currentUser.UserId != streamerId
            && !await roleService.HasRole(
                streamerId,
                currentUser.UserId,
                Roles.Enums.Permissions.Revenue
            )
        )
        {
            throw new ForbiddenException("You are not authorized to perform this action.");
        }

        var partner = await context
            .Partners.Include(x => x.Streamer)
            .FirstOrDefaultAsync(s => s.StreamerId == streamerId, cancellationToken);

        if (partner == null)
        {
            throw new StreamerNotFoundException(streamerId);
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
