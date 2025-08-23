namespace Streamers.Features.Chats.Dtos;

public class ChatDto
{
    public required Guid Id { get; set; }
    public required Guid SettingsId { get; set; }
    public required Guid? PinnedMessageId { get; set; }
    public required string StreamerId { get; set; }
}
