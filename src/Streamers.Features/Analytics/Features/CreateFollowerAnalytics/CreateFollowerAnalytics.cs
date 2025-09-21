using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Analytics.Enums;
using Streamers.Features.Analytics.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Analytics.Features.CreateFollowerAnalytics;

public record CreateFollowerAnalyticsResponse();

public record CreateFollowerAnalytics() : IRequest<CreateFollowerAnalyticsResponse>;

public class CreateFollowerAnalyticsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<CreateFollowerAnalytics, CreateFollowerAnalyticsResponse>
{
    public async Task<CreateFollowerAnalyticsResponse> Handle(
        CreateFollowerAnalytics request,
        CancellationToken cancellationToken
    )
    {
        var followers = await streamerDbContext
            .Streamers.Select(x => new { x.Id, x.Followers })
            .ToListAsync(cancellationToken: cancellationToken);
        var analyticsItems = followers.Select(x => new AnalyticsItem(
            x.Followers,
            DateTime.UtcNow,
            AnalyticsItemType.Follower,
            x.Id
        ));
        await streamerDbContext.AnalyticsItems.AddRangeAsync(analyticsItems, cancellationToken);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new CreateFollowerAnalyticsResponse();
    }
}
