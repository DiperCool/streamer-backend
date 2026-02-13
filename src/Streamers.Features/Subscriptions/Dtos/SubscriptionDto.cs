using Streamers.Features.Subscriptions.Models;

namespace Streamers.Features.Subscriptions.Dtos;

public record SubscriptionDto
{
    public required Guid Id { get; init; }
    public required string UserId { get; init; }
    public required string StreamerId { get; init; }
    public required SubscriptionStatus Status { get; init; }
    public required DateTime CurrentPeriodEnd { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required string Title { get; init; }
}
