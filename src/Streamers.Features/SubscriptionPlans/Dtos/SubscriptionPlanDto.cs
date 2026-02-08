namespace Streamers.Features.SubscriptionPlans.Dtos;

public record SubscriptionPlanDto
{
    public required Guid Id { get; init; }
    public required string StreamerId { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }
}
