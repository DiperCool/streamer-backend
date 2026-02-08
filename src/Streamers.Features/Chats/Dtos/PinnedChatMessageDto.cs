namespace Streamers.Features.Chats.Dtos;

public class PinnedChatMessageDto
{
    public required Guid Id { get; set; }
    public required Guid MessageId { get; set; }
    public required string PinnedById { get; set; }
    public required DateTime CreatedAt { get; set; }
}
