using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Streams.BackgroundServices;

public class ViewerSyncJob(StreamerDbContext context, IConnectionMultiplexer redis)
{
    public async Task Run(CancellationToken cancellationToken = default)
    {
        var db = redis.GetDatabase();

        var activeStreams = await context
            .Streams.Where(s => s.Active)
            .ToListAsync(cancellationToken);

        if (activeStreams.Count == 0)
            return;

        var keys = activeStreams.Select(s => (RedisKey)$"stream-viewers-{s.Id}").ToArray();

        var values = await db.StringGetAsync(keys);

        for (int i = 0; i < activeStreams.Count; i++)
        {
            if (!values[i].HasValue || !long.TryParse(values[i].ToString(), out var viewers))
                continue;

            if (activeStreams[i].CurrentViewers != viewers)
            {
                activeStreams[i].SetCurrentViewers(viewers);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
