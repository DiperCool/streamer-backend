namespace Streamers.Features.Followers.Dtos;

public class StreamerFollowerDto
{
    public required string Id { get; set; }
    public required string? UserName { get; set; }
    public required string? Avatar { get; set; }
    public required bool IsLive { get; set; }
    public required Guid? CurrentStreamId { get; set; }
}
