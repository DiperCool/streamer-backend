using Streamers.Features.Subscriptions.Models;

namespace Streamers.Features.Subscriptions.Dtos;

public class SubscriptionDto
{
    public required Guid Id { get; set; }
    public required string UserId { get; set; }
    public required string StreamerId { get; set; }
    public required SubscriptionStatus Status { get; set; }
    public required DateTime CurrentPeriodEnd { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required string Title { get; set; }
    public required int CurrentStreak { get; set; }
}
