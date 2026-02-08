using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Analytics.Enums;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Analytics.Features.GetOverviewAnalytics;

public record OverviewAnalyticsItem(AnalyticsItemType Type, double Value);

public record GetOverviewAnalyticsResponse(int Days, List<OverviewAnalyticsItem> Items);

public record GetOverviewAnalytics(string BroadcasterId, DateTime From, DateTime To)
    : IRequest<GetOverviewAnalyticsResponse>;

public class GetOverviewAnalyticsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetOverviewAnalytics, GetOverviewAnalyticsResponse>
{
    public async Task<GetOverviewAnalyticsResponse> Handle(
        GetOverviewAnalytics request,
        CancellationToken cancellationToken
    )
    {
        var dbItems = await streamerDbContext
            .AnalyticsItems.Where(a =>
                a.StreamerId == request.BroadcasterId
                && a.CreatedAt >= request.From
                && a.CreatedAt <= request.To
            )
            .GroupBy(a => a.Type)
            .Select(g => new
            {
                Type = g.Key,
                Value = g.Key == AnalyticsItemType.StreamTime
                    ? g.Sum(a => a.Value)
                    : g.Average(a => a.Value),
            })
            .ToListAsync(cancellationToken);

        var allTypes = Enum.GetValues(typeof(AnalyticsItemType)).Cast<AnalyticsItemType>();
        var overviewItems = allTypes
            .Select(t => new OverviewAnalyticsItem(
                t,
                dbItems.FirstOrDefault(x => x.Type == t)?.Value ?? 0
            ))
            .ToList();

        var days = (request.To.Date - request.From.Date).Days + 1;

        return new GetOverviewAnalyticsResponse(days, overviewItems);
    }
}
