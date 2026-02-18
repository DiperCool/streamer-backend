using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Analytics.Enums;
using Streamers.Features.Analytics.Models;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Subscriptions.Models;

namespace Streamers.Features.Analytics.Features.CreateSubscriptionsAnalytics;

public record CreateSubscriptionsAnalyticsResponse();

public record CreateSubscriptionsAnalytics : IRequest<CreateSubscriptionsAnalyticsResponse>;

public class CreateSubscriptionsAnalyticsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<CreateSubscriptionsAnalytics, CreateSubscriptionsAnalyticsResponse>
{
    public async Task<CreateSubscriptionsAnalyticsResponse> Handle(
        CreateSubscriptionsAnalytics request,
        CancellationToken cancellationToken
    )
    {
        var subscriptions = await streamerDbContext
            .Subscriptions.GroupBy(x => x.StreamerId)
            .Select(x => new { StreamerId = x.Key, Count = x.Count() })
            .ToListAsync(cancellationToken: cancellationToken);
        var items = subscriptions.Select(x => new AnalyticsItem(
            x.Count,
            DateTime.UtcNow,
            AnalyticsItemType.Subscriptions,
            x.StreamerId
        ));
        await streamerDbContext.AnalyticsItems.AddRangeAsync(items, cancellationToken);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new CreateSubscriptionsAnalyticsResponse();
    }
}
