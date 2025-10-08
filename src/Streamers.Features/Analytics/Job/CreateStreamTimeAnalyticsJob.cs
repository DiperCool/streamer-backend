using Shared.Abstractions.Cqrs;
using Streamers.Features.Analytics.Features.CreateStreamTimeAnalytics;

namespace Streamers.Features.Analytics.Job;

public class CreateStreamTimeAnalyticsJob(IMediator mediator)
{
    public async Task Run(CancellationToken cancellationToken = default)
    {
        await mediator.Send(new CreateStreamTimeAnalytics(), cancellationToken);
    }
}
