using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using StackExchange.Redis;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Subscriptions.Features.GetStreamerSubscriptionsStats;

public record GetStreamerSubscriptionsStatsResponse(
    int ActiveSubscriptionsCount,
    decimal FuturePayoutAmount
);

public record GetStreamerSubscriptionsStats(string StreamerId)
    : IRequest<GetStreamerSubscriptionsStatsResponse>;

public class GetStreamerSubscriptionsStatsHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser,
    IRoleService roleService,
    IStripeService stripeService,
    IConnectionMultiplexer redis
) : IRequestHandler<GetStreamerSubscriptionsStats, GetStreamerSubscriptionsStatsResponse>
{
    private readonly IDatabase _db = redis.GetDatabase();
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public async Task<GetStreamerSubscriptionsStatsResponse> Handle(
        GetStreamerSubscriptionsStats request,
        CancellationToken cancellationToken
    )
    {
        if (!await roleService.HasRole(request.StreamerId, currentUser.UserId, Permissions.Revenue))
        {
            throw new UnauthorizedAccessException();
        }

        var activeSubscriptionsCount = await streamerDbContext
            .Subscriptions.Where(s => s.StreamerId == request.StreamerId)
            .CountAsync(cancellationToken);

        decimal futurePayoutAmount;
        string cacheKey = $"stripe_balance_{request.StreamerId}";
        string? cachedBalance = await _db.StringGetAsync(cacheKey);

        if (cachedBalance != null)
        {
            futurePayoutAmount = JsonSerializer.Deserialize<decimal>(cachedBalance);
        }
        else
        {
            futurePayoutAmount = await stripeService.GetCurrentBalanceAsync(cancellationToken);
            await _db.StringSetAsync(
                cacheKey,
                JsonSerializer.Serialize(futurePayoutAmount),
                CacheDuration
            );
        }

        return new GetStreamerSubscriptionsStatsResponse(
            activeSubscriptionsCount,
            futurePayoutAmount
        );
    }
}
