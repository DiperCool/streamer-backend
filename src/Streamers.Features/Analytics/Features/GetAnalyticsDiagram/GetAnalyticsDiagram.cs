using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
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

public record AnalyticsDiagramItem(string Title, double Value);

public record GetAnalyticsDiagram(
    string BroadcasterId,
    AnalyticsItemType Type,
    DateTime From,
    DateTime To,
    AnalyticsDiagramType AnalyticsDiagramType
) : IRequest<List<AnalyticsDiagramItem>>;

public class GetAnalyticsDiagramHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetAnalyticsDiagram, List<AnalyticsDiagramItem>>
{
    public async Task<List<AnalyticsDiagramItem>> Handle(
        GetAnalyticsDiagram request,
        CancellationToken cancellationToken
    )
    {
        var diagramType = request.AnalyticsDiagramType.ToString();

        var dateFormat = diagramType switch
        {
            "Day" => "DD Mon",
            "Week" => "DD Mon",
            "Month" => "Month YYYY",
            _ => "DD Mon",
        };

        var intervalSql = diagramType switch
        {
            "Day" => "1 day",
            "Week" => "1 week",
            "Month" => "1 month",
            _ => "1 day",
        };

        var sql = $"""
                WITH TimeSeries AS (
                    SELECT generate_series(
                        @from::timestamp,
                        @to::timestamp,
                        @interval::interval
                    ) AS Title
                )
                SELECT
                    TO_CHAR(t.Title, @dateFormat) AS Title,
                    COALESCE(AVG(ai."Value"), 0)::bigint AS Value
                FROM TimeSeries t
                LEFT JOIN "AnalyticsItems" ai
                    ON ai."Type" = @type
                    AND ai."StreamerId" = @broadcasterId
                    AND ai."CreatedAt" >= t.Title
                    AND ai."CreatedAt" < t.Title + @interval::interval
                GROUP BY t.Title
                ORDER BY t.Title;
            """;

        var result = await streamerDbContext
            .Database.SqlQueryRaw<AnalyticsDiagramItem>(
                sql,
                new Npgsql.NpgsqlParameter("from", request.From),
                new Npgsql.NpgsqlParameter("to", request.To),
                new Npgsql.NpgsqlParameter("interval", intervalSql),
                new Npgsql.NpgsqlParameter("dateFormat", dateFormat),
                new Npgsql.NpgsqlParameter("type", (int)request.Type),
                new Npgsql.NpgsqlParameter("broadcasterId", request.BroadcasterId)
            )
            .ToListAsync(cancellationToken);

        return result;
    }
}
