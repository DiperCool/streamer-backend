namespace Streamers.Features.Notifications.Dtos;

public class NotificationSettingsDto
{
    public required Guid Id { get; set; }
    public required bool StreamerLive { get; set; }
    public required bool UserFollowed { get; set; }
}
