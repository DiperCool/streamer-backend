using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using StackExchange.Redis;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Payments.Features.GenerateOnboardingLink;

public record OnboardingLinkResponse(string OnboardingLink);

public record GenerateOnboardingLinkQuery : IRequest<OnboardingLinkResponse>;

public class GenerateOnboardingLinkQueryHandler(
    StreamerDbContext context,
    ICurrentUser currentUser,
    IStripeService stripeService,
    IConnectionMultiplexer redis
) : IRequestHandler<GenerateOnboardingLinkQuery, OnboardingLinkResponse>
{
    private readonly IDatabase _redisDb = redis.GetDatabase();

    public async Task<OnboardingLinkResponse> Handle(
        GenerateOnboardingLinkQuery request,
        CancellationToken cancellationToken
    )
    {
        var streamerId = currentUser.UserId;
        var cacheKey = $"onboarding-link:{streamerId}";

        var cachedLink = await _redisDb.StringGetAsync(cacheKey);
        if (cachedLink.HasValue)
        {
            return new OnboardingLinkResponse(cachedLink.ToString());
        }

        var partner = await context.Partners.FirstOrDefaultAsync(
            p => p.StreamerId == streamerId,
            cancellationToken
        );

        if (partner is null)
        {
            throw new InvalidOperationException($"Partner not found: {streamerId}");
        }

        if (string.IsNullOrEmpty(partner.StripeAccountId))
        {
            throw new InvalidOperationException(
                $"Partner {partner.Id} has no Stripe Account ID and cannot generate an onboarding link."
            );
        }

        var (onboardingUrl, expiresAt) = await stripeService.CreateAccountLinkAsync(
            partner.StripeAccountId,
            cancellationToken
        );

        var expiryTime = expiresAt - DateTime.UtcNow;
        await _redisDb.StringSetAsync(cacheKey, onboardingUrl, expiryTime);

        return new OnboardingLinkResponse(onboardingUrl);
    }
}
