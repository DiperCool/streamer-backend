using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Notifications.Exceptions;

public class NotificationSettingsNotFoundException() : NotFoundException("Notification settings not found.");