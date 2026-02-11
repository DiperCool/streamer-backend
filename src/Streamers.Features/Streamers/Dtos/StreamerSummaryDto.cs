namespace Streamers.Features.Streamers.Dtos;

public class StreamerSummaryDto
{
    public required string Id { get; set; }
    public required string? UserName { get; set; }
    public required string? Avatar { get; set; }
    public required long Followers { get; set; }
    public required bool IsLive { get; set; }
    public required bool SubscriptionEnabled { get; set; }
}
