namespace Streamers.Features.Chats.Dtos;

public class BannedUserDto
{
    public required Guid Id { get; set; }
    public required string UserId { get; set; }

    public required string BannedById { get; set; }
    public required DateTime BannedAt { get; set; }
    public required DateTime BannedUntil { get; set; }

    public required string Reason { get; set; }
}
