using Streamers.Features.Bots.Enums;

namespace Streamers.Features.Bots.Dtos;

public class BotDto
{
    public required Guid Id { get; set; }
    public required string StreamerId { get; set; }
    public required BotState State { get; set; }
    public required string StreamVideoUrl { get; set; }
}
