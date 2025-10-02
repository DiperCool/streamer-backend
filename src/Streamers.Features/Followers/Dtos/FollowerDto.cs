namespace Streamers.Features.Followers.Dtos;

public class FollowerDto
{
    public required string FollowerStreamerId { get; set; }
    public required DateTime FollowedAt { get; set; }
}
