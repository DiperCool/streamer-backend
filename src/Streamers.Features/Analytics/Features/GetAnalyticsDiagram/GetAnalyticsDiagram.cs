using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Analytics.Enums;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Analytics.Features.GetAnalyticsDiagram;

public enum AnalyticsDiagramType
{
    Day,
    Week,
    Month,
}

public record AnalyticsDiagramItem(string Title, long Value);

public record GetAnalyticsDiagram(
    AnalyticsItemType Type,
    DateTime From,
    DateTime To,
    AnalyticsDiagramType AnalyticsDiagramType
) : IRequest<List<AnalyticsDiagramItem>>;

public class GetAnalyticsDiagramHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser
) : IRequestHandler<GetAnalyticsDiagram, List<AnalyticsDiagramItem>>
{
    public async Task<List<AnalyticsDiagramItem>> Handle(
        GetAnalyticsDiagram request,
        CancellationToken cancellationToken
    )
    {
        var streamerId = currentUser.UserId;
        var diagramType = request.AnalyticsDiagramType.ToString();

        var result = await streamerDbContext
            .Database.SqlQuery<AnalyticsDiagramItem>(
                $"""
                WITH TimeSeries AS (
                    SELECT generate_series(
                        {request.From}::timestamp,
                        {request.To}::timestamp,
                        CASE {diagramType}
                            WHEN 'Day' THEN '1 day'::interval
                            WHEN 'Week' THEN '1 week'::interval
                            WHEN 'Month' THEN '1 month'::interval
                        END
                    ) AS Title
                )
                SELECT
                    Title,
                    COALESCE(SUM(ai.Value), 0) AS Value
                FROM TimeSeries t
                LEFT JOIN AnalyticsItems ai
                    ON ai.Type = {(int)request.Type}
                    AND ai.StreamerId = {streamerId}
                    AND ai.CreatedAt >= t.Title
                    AND ai.CreatedAt < t.Title +
                        CASE {diagramType}
                            WHEN 'Day' THEN '1 day'::interval
                            WHEN 'Week' THEN '1 week'::interval
                            WHEN 'Month' THEN '1 month'::interval
                        END
                GROUP BY Title
                ORDER BY Title;
                """
            )
            .ToListAsync(cancellationToken);

        return result;
    }
}
