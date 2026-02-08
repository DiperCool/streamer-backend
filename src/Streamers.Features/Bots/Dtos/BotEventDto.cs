using Streamers.Features.Bots.Enums;
using Streamers.Features.Bots.Models;

namespace Streamers.Features.Bots.Dtos;

public class BotEventDto
{
    public required Guid Id { get; set; }
    public required string StreamerId { get; set; }
    public required BotState State { get; set; }
    public required string StreamVideoUrl { get; set; }
    public required string StreamKey { get; set; }

    public static BotEventDto Create(Bot bot)
    {
        return new BotEventDto
        {
            Id = bot.Id,
            StreamerId = bot.StreamerId,
            State = bot.State,
            StreamVideoUrl = bot.StreamVideoUrl,
            StreamKey = bot.Streamer.StreamSettings.StreamKey,
        };
    }
}
