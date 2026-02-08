using Streamers.Features.Notifications.Models;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Notifications.Services;

public interface INotificationService
{
    Task CreateLiveStartedNotification(List<string> userIds, Streamer streamer);
    Task CreateUserFollowedNotification(Streamer follower, Streamer streamer);
}

public class NotificationService(StreamerDbContext streamerDbContext) : INotificationService
{
    public async Task CreateLiveStartedNotification(List<string> userIds, Streamer streamer)
    {
        var notifications = userIds
            .Select(x => new LiveStartedNotification(x, DateTime.UtcNow, streamer))
            .ToList();

        await streamerDbContext.Notifications.AddRangeAsync(notifications);
    }

    public async Task CreateUserFollowedNotification(Streamer follower, Streamer streamer)
    {
        var notification = new UserFollowedNotification(streamer.Id, DateTime.UtcNow, follower);
        await streamerDbContext.Notifications.AddAsync(notification);
        await streamerDbContext.SaveChangesAsync();
    }
}
