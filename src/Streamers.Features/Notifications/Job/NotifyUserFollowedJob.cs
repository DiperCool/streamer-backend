using Microsoft.EntityFrameworkCore;
using Streamers.Features.Notifications.Services;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Notifications.Job;

public class NotifyUserFollowedJob(
    StreamerDbContext streamerDbContext,
    INotificationService notificationService
)
{
    public async Task NotifyUserFollowed(string streamerId, string followerId)
    {
        var streamer = await streamerDbContext.Streamers.FirstOrDefaultAsync(x =>
            x.Id == streamerId && x.NotificationSettings.UserFollowed
        );
        if (streamer == null)
        {
            return;
        }
        var follower = await streamerDbContext.Streamers.FirstOrDefaultAsync(x =>
            x.Id == followerId && x.NotificationSettings.UserFollowed
        );
        if (follower == null)
        {
            return;
        }
        await notificationService.CreateUserFollowedNotification(follower, streamer);
        await streamerDbContext.SaveChangesAsync();
    }
}
