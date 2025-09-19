using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Notifications.Models;

public record NotificationCreated(Notification Notification) : IDomainEvent;

public abstract class Notification : Entity
{
    public string UserId { get; set; }

    public Streamer User { get; set; }
    public DateTime CreatedAt { get; set; }

    public bool Seen { get; set; }
    public string Discriminator { get; set; }

    protected Notification() { }

    public Notification(string streamerId, DateTime createdAt)
    {
        UserId = streamerId;

        CreatedAt = createdAt;
        Raise(new NotificationCreated(this));
    }
}
