using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Notifications.Exceptions;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Notifications.Features.EditNotificationSettings;

public record EditNotificationSettingsResponse(Guid Id);

public record EditNotificationSettings(bool StreamerLive, bool UserFollowed)
    : IRequest<EditNotificationSettingsResponse>;

public class EditNotificationSettingsHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser
) : IRequestHandler<EditNotificationSettings, EditNotificationSettingsResponse>
{
    public async Task<EditNotificationSettingsResponse> Handle(
        EditNotificationSettings request,
        CancellationToken cancellationToken
    )
    {
        var settings = await streamerDbContext.NotificationSettings.FirstOrDefaultAsync(
            x => x.Streamer.Id == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (settings == null)
        {
            throw new NotificationSettingsNotFoundException();
        }

        settings.StreamerLive = request.StreamerLive;
        settings.UserFollowed = request.UserFollowed;
        streamerDbContext.NotificationSettings.Update(settings);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new EditNotificationSettingsResponse(settings.Id);
    }
}
