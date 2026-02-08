using Streamers.Features.Streamers.Enums;

namespace Streamers.Features.Payments.Dtos;

public record PartnerDto
{
    public required Guid Id { get; init; }
    public required string StreamerId { get; init; }
    public required string? StripeAccountId { get; init; }
    public required StripeOnboardingStatus StripeOnboardingStatus { get; init; }
}
