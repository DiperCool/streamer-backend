using Microsoft.EntityFrameworkCore;
using Streamers.Features.Notifications.Services;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Notifications.Job;

public class NotifyFollowersStreamerLiveJob(
    StreamerDbContext streamerDbContext,
    INotificationService notificationService
)
{
    public async Task NotifyFollowersStreamerLive(string streamerId)
    {
        var streamer = await streamerDbContext.Streamers.FirstOrDefaultAsync(x =>
            x.Id == streamerId
        );
        if (streamer == null || !streamer.IsLive)
        {
            return;
        }

        var followers = await streamerDbContext
            .Followers.Where(x =>
                x.StreamerId == streamer.Id && x.Streamer.NotificationSettings.StreamerLive
            )
            .Select(x => x.FollowerStreamerId)
            .ToListAsync();
        await streamerDbContext
            .Streamers.Where(x => followers.Contains(x.Id))
            .ExecuteUpdateAsync(x => x.SetProperty(a => a.HasUnreadNotifications, true));
        await notificationService.CreateLiveStartedNotification(followers, streamer);
        await streamerDbContext.SaveChangesAsync();
    }
}
