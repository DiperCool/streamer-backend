using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Analytics.Features.GetAnalyticsDiagram;
using Streamers.Features.Analytics.Features.GetOverviewAnalytics;

namespace Streamers.Features.Analytics.Graphql;

[Authorize]
[QueryType]
public static partial class AnalyticsQuery
{
    public static async Task<GetOverviewAnalyticsResponse> GetOverviewAnalytics(
        GetOverviewAnalytics param,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        return await mediator.Send(param, cancellationToken);
    }

    public static async Task<List<AnalyticsDiagramItem>> GetAnalyticsDiagram(
        GetAnalyticsDiagram param,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        return await mediator.Send(param, cancellationToken);
    }
}
