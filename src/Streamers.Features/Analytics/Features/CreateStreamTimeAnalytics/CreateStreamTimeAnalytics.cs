using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Analytics.Enums;
using Streamers.Features.Analytics.Models;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Models;
using Stream = Streamers.Features.Streams.Models.Stream;

namespace Streamers.Features.Analytics.Features.CreateStreamTimeAnalytics;

public record CreateStreamTimeAnalyticsResponse();

public record CreateStreamTimeAnalytics() : IRequest<CreateStreamTimeAnalyticsResponse>;

public class CreateStreamTimeAnalyticsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<CreateStreamTimeAnalytics, CreateStreamTimeAnalyticsResponse>
{
    public async Task<CreateStreamTimeAnalyticsResponse> Handle(
        CreateStreamTimeAnalytics request,
        CancellationToken cancellationToken
    )
    {
        List<Streamer> streamers = await streamerDbContext
            .Streamers.Where(x => x.IsLive)
            .ToListAsync(cancellationToken: cancellationToken);
        var items = new List<AnalyticsItem>();
        const long minutes = 1;
        items.AddRange(
            streamers.Select(x => new AnalyticsItem(
                minutes,
                DateTime.UtcNow,
                AnalyticsItemType.StreamTime,
                x.Id
            ))
        );
        await streamerDbContext.AnalyticsItems.AddRangeAsync(items, cancellationToken);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new CreateStreamTimeAnalyticsResponse();
    }
}
