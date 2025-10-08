using Hangfire;
using Microsoft.Extensions.Hosting;
using Streamers.Features.Analytics.Features.CreateStreamViewerAnalytics;
using Streamers.Features.Analytics.Job;
using Streamers.Features.Streams.BackgroundServices;

namespace Streamers.Features.Shared.Hangfire;

public class RecurringJobsHostedService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        RecurringJob.AddOrUpdate<ViewerSyncJob>(
            nameof(ViewerSyncJob),
            job => job.Run(CancellationToken.None),
            "*/5 * * * * *"
        );
        RecurringJob.AddOrUpdate<CreateStreamViewerAnalyticsJob>(
            nameof(CreateStreamViewerAnalyticsJob),
            job => job.Run(CancellationToken.None),
            "* * * * *"
        );
        RecurringJob.AddOrUpdate<CreateFollowerAnalyticsJob>(
            nameof(CreateFollowerAnalyticsJob),
            job => job.Run(CancellationToken.None),
            "* * * * *"
        );
        RecurringJob.AddOrUpdate<CreateStreamTimeAnalyticsJob>(
            nameof(CreateStreamTimeAnalyticsJob),
            job => job.Run(CancellationToken.None),
            "* * * * *"
        );

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
