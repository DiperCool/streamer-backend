using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Notifications.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Notifications.Features.GetNotificationSettings;

public record GetNotificationSettings : IRequest<NotificationSettingsDto>;

public class GetNotificationSettingsHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser
) : IRequestHandler<GetNotificationSettings, NotificationSettingsDto>
{
    public async Task<NotificationSettingsDto> Handle(
        GetNotificationSettings request,
        CancellationToken cancellationToken
    )
    {
        var settings = await streamerDbContext.NotificationSettings.FirstOrDefaultAsync(
            x => x.Streamer.Id == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (settings == null)
        {
            throw new InvalidOperationException("Notification settings not found");
        }

        return new NotificationSettingsDto
        {
            Id = settings.Id,
            StreamerLive = settings.StreamerLive,
            UserFollowed = settings.UserFollowed,
        };
    }
}
