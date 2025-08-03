using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace streamer.ServiceDefaults;

public class MigrationWorker<TContext>(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<MigrationWorker<TContext>> logger)
    : IHostedService
    where TContext : DbContext
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Migration worker started.");

        using var serviceScope = serviceScopeFactory.CreateScope();
        var catalogDbContext = serviceScope.ServiceProvider.GetRequiredService<TContext>();

        logger.LogInformation("Updating catalog database...");

        await catalogDbContext.Database.MigrateAsync(cancellationToken: cancellationToken);

        logger.LogInformation("Catalog database Updated");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Migration worker stopped.");

        return Task.CompletedTask;
    }
}
