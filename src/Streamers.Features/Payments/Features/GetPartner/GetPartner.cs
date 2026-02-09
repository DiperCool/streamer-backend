using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Payments.Dtos;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Enums;

namespace Streamers.Features.Payments.Features.GetPartner;

public record GetPartner(string StreamerId) : IRequest<PartnerDto>;

public class GetPartnerHandler(
    StreamerDbContext context,
    ICurrentUser currentUser,
    IRoleService roleService
) : IRequestHandler<GetPartner, PartnerDto>
{
    public async Task<PartnerDto> Handle(GetPartner request, CancellationToken cancellationToken)
    {
        if (
            currentUser.UserId != request.StreamerId
            && !await roleService.HasRole(
                request.StreamerId,
                currentUser.UserId,
                Roles.Enums.Permissions.Revenue
            )
        )
        {
            throw new UnauthorizedAccessException(
                "You are not authorized to view this partner's information."
            );
        }

        var partner = await context
            .Partners.Include(x => x.Streamer)
            .FirstOrDefaultAsync(p => p.StreamerId == request.StreamerId, cancellationToken);

        if (partner == null)
        {
            throw new InvalidOperationException(
                $"Partner with StreamerId {request.StreamerId} not found."
            );
        }

        return new PartnerDto
        {
            Id = partner.Id,
            StreamerId = partner.StreamerId,
            StripeAccountId = partner.StripeAccountId,
            StripeOnboardingStatus = partner.StripeOnboardingStatus,
        };
    }
}
