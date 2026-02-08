using Streamers.Features.Chats.Enums;

namespace Streamers.Features.Chats.Dtos;

public class ChatMessageDto
{
    public required Guid Id { get; set; }
    public required ChatMessageType Type { get; set; }
    public required string SenderId { get; set; }
    public required string Message { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required bool IsDeleted { get; set; }
    public required bool IsActive { get; set; }
    public required Guid? ReplyId { get; set; }
}
