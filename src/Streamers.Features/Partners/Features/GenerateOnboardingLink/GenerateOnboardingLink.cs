using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using StackExchange.Redis;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Partners.Exceptions;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.PaymentMethods.Exceptions;

namespace Streamers.Features.Partners.Features.GenerateOnboardingLink;

public record OnboardingLinkResponse(string OnboardingLink);

public record GenerateOnboardingLinkQuery(string StreamerId) : IRequest<OnboardingLinkResponse>;

public class GenerateOnboardingLinkQueryHandler(
    StreamerDbContext context,
    ICurrentUser currentUser,
    IStripeService stripeService,
    IRoleService roleService
) : IRequestHandler<GenerateOnboardingLinkQuery, OnboardingLinkResponse>
{
    public async Task<OnboardingLinkResponse> Handle(
        GenerateOnboardingLinkQuery request,
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

        var partner = await context.Partners.FirstOrDefaultAsync(
            p => p.StreamerId == streamerId,
            cancellationToken
        );

        if (partner is null)
        {
            throw new PartnerNotFoundException(streamerId);
        }

        if (string.IsNullOrEmpty(partner.StripeAccountId))
        {
            throw new StripeErrorException($"Partner {partner.Id} has no Stripe Account ID and cannot generate an onboarding link.");
        }

        var (onboardingUrl, expiresAt) = await stripeService.CreateAccountLinkAsync(
            partner.StripeAccountId,
            cancellationToken
        );

        return new OnboardingLinkResponse(onboardingUrl);
    }
}
