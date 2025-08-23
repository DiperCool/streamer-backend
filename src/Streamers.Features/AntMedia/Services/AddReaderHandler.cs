using StackExchange.Redis;

namespace Streamers.Features.AntMedia.Services;

public class AddReaderHandler(IConnectionMultiplexer redis)
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task HandleAsync(Guid streamId)
    {
        string key = $"stream-viewers-{streamId}";
        await _db.StringIncrementAsync(key);
    }
}
