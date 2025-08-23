using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Streamers.Features.Shared.Persistance;

public class ViewerSyncJob
{
    private readonly StreamerDbContext _context;
    private readonly IConnectionMultiplexer _redis;

    public ViewerSyncJob(StreamerDbContext context, IConnectionMultiplexer redis)
    {
        _context = context;
        _redis = redis;
    }

    public async Task Run(CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();

        var activeStreams = await _context
            .Streams.Where(s => s.Active)
            .ToListAsync(cancellationToken);

        if (activeStreams.Count == 0)
            return;

        var keys = activeStreams.Select(s => (RedisKey)$"stream-viewers-{s.Id}").ToArray();

        var values = await db.StringGetAsync(keys);

        for (int i = 0; i < activeStreams.Count; i++)
        {
            if (!values[i].HasValue || !long.TryParse(values[i], out var viewers))
                continue;

            if (activeStreams[i].CurrentViewers != viewers)
                activeStreams[i].SetCurrentViewers(viewers);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
