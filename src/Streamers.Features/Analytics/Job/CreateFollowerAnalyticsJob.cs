using Shared.Abstractions.Cqrs;
using Streamers.Features.Analytics.Features.CreateFollowerAnalytics;

namespace Streamers.Features.Analytics.Job;

public class CreateFollowerAnalyticsJob(IMediator mediator)
{
    public async Task Run(CancellationToken cancellationToken = default)
    {
        await mediator.Send(new CreateFollowerAnalytics(), cancellationToken);
    }
}
