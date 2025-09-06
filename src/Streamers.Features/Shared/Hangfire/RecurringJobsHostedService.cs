using Hangfire;
using Microsoft.Extensions.Hosting;
using Streamers.Features.Streams.BackgroundServices;

namespace Streamers.Features.Shared.Hangfire;

public class RecurringJobsHostedService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        RecurringJob.AddOrUpdate<ViewerSyncJob>(
            "viewer-sync-job",
            job => job.Run(CancellationToken.None),
            "*/5 * * * * *"
        );

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
