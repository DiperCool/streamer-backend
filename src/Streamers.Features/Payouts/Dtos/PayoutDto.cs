namespace Streamers.Features.Payouts.Dtos;

public record PayoutDto
{
    public required Guid Id { get; init; }
    public required string StreamerId { get; init; }
    public required string StripePayoutId { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required string Status { get; init; }
    public required DateTime ArrivalDate { get; init; }
    public string? FailureMessage { get; init; }
    public required DateTime CreatedAt { get; init; }
}
