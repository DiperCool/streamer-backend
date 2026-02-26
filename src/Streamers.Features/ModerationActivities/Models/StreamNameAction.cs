using Streamers.Features.Streamers.Models;

namespace Streamers.Features.ModerationActivities.Models;

public class StreamNameAction : ModeratorActionType
{
    public string NewStreamName { get; protected set; } = default!;

    public StreamNameAction(string moderatorId, string streamerId, string newStreamName)
        : base("StreamName", moderatorId, streamerId)
    {
        NewStreamName = newStreamName;
    }

    private StreamNameAction()
        : base() { }
}
