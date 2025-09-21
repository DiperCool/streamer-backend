using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Analytics.Enums;
using Streamers.Features.Analytics.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Analytics.Features.CreateStreamViewerAnalytics;

public record CreateStreamViewerAnalyticsResponse();

public record CreateStreamViewerAnalytics() : IRequest<CreateStreamViewerAnalyticsResponse>;

public class CreateStreamViewerAnalyticsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<CreateStreamViewerAnalytics, CreateStreamViewerAnalyticsResponse>
{
    public async Task<CreateStreamViewerAnalyticsResponse> Handle(
        CreateStreamViewerAnalytics request,
        CancellationToken cancellationToken
    )
    {
        var streams = await streamerDbContext
            .Streams.AsNoTracking()
            .Where(x => x.Active)
            .Select(x => new { x.CurrentViewers, x.StreamerId })
            .ToListAsync(cancellationToken: cancellationToken);
        var items = streams.Select(x => new AnalyticsItem(
            x.CurrentViewers,
            DateTime.UtcNow,
            AnalyticsItemType.StreamViewers,
            x.StreamerId
        ));
        await streamerDbContext.AnalyticsItems.AddRangeAsync(items, cancellationToken);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new CreateStreamViewerAnalyticsResponse();
    }
}
