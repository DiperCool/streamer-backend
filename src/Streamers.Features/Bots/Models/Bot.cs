using Shared.Abstractions.Domain;
using Streamers.Features.Bots.Enums;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Bots.Models;

public class Bot : Entity
{
    protected Bot() { }

    public Bot(Streamer streamer, BotState state, string streamVideoUrl)
    {
        StreamerId = streamer.Id;
        Streamer = streamer;
        State = state;
        StreamVideoUrl = streamVideoUrl;
    }

    public Streamer Streamer { get; private set; }
    public string StreamerId { get; private set; }
    public BotState State { get; private set; }
    public string StreamVideoUrl { get; private set; }

    public void Edit(BotState state, string streamVideoUrl)
    {
        State = state;
        StreamVideoUrl = streamVideoUrl;
    }
}
