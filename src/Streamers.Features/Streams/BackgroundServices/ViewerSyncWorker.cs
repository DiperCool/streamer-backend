using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Streams.BackgroundServices;

public class ViewerSyncWorker(IServiceProvider services, IConnectionMultiplexer redis)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var db = redis.GetDatabase();

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);

            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StreamerDbContext>();

            var activeStreams = await context
                .Streams.Where(s => s.Active)
                .ToListAsync(stoppingToken);

            foreach (var stream in activeStreams)
            {
                string redisKey = $"stream-viewers-{stream.Id}";

                var value = await db.StringGetAsync(redisKey);
                if (value.HasValue && long.TryParse(value.ToString(), out var viewers))
                {
                    stream.SetCurrentViewers(viewers);
                }
            }

            await context.SaveChangesAsync(stoppingToken);
        }
    }
}
