namespace Streamers.Features.Streams.Dtos;

public class StreamParticipantDto
{
    public string Role { get; set; }
    public bool CanWriteChat { get; set; }
    public string CantWriteChatReason { get; set; }
}
