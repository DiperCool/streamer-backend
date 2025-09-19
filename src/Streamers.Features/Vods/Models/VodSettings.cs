using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Vods.Models;

public class VodSettings : Entity
{
    public string StreamerId { get; set; }

    public Streamer Streamer { get; set; }

    public bool VodEnabled { get; set; }
}
