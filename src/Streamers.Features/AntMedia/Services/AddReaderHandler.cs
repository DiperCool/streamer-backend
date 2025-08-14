using StackExchange.Redis;

namespace Streamers.Features.AntMedia.Services;

public class AddReaderHandler(IConnectionMultiplexer redis) : IAntMediaWebhookHandler
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task HandleAsync(AntMediaWebhookPayload payload)
    {
        string key = $"stream-viewers-{payload.Id}";
        await _db.StringIncrementAsync(key);
    }
}
