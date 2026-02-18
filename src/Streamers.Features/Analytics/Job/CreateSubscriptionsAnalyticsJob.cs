using Shared.Abstractions.Cqrs;
using Streamers.Features.Analytics.Features.CreateSubscriptionsAnalytics;

namespace Streamers.Features.Analytics.Job;

public class CreateSubscriptionsAnalyticsJob(IMediator mediator)
{
    public async Task Run(CancellationToken cancellationToken = default)
    {
        await mediator.Send(new CreateSubscriptionsAnalytics(), cancellationToken);
    }
}
