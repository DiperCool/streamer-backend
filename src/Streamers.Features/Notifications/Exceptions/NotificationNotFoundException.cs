using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.Notifications.Exceptions;

public class NotificationNotFoundException(Guid id) : NotFoundException($"Notification with id '{id}' not found.");