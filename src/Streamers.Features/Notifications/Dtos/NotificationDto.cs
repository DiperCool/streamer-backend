namespace Streamers.Features.Notifications.Dtos;

public class NotificationDto
{
    public required Guid Id { get; set; }
    public required DateTime CreatedAt { get; set; }

    public required bool Seen { get; set; }
    public required string Discriminator { get; set; }
    public required string? StreamerId { get; set; }
}
