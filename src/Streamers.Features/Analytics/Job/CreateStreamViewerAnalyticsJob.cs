using Shared.Abstractions.Cqrs;
using Streamers.Features.Analytics.Features.CreateStreamViewerAnalytics;

namespace Streamers.Features.Analytics.Job;

public class CreateStreamViewerAnalyticsJob(IMediator mediator)
{
    public async Task Run(CancellationToken cancellationToken = default)
    {
        await mediator.Send(new CreateStreamViewerAnalytics(), cancellationToken);
    }
}
