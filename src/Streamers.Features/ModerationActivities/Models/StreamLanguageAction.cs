using Streamers.Features.Streamers.Models;

namespace Streamers.Features.ModerationActivities.Models;

public class StreamLanguageAction : ModeratorActionType
{
    public string NewLanguage { get; protected set; } = default!;

    public StreamLanguageAction(string moderatorId, string streamerId, string newLanguage)
        : base("StreamLanguage", moderatorId, streamerId)
    {
        NewLanguage = newLanguage;
    }

    private StreamLanguageAction()
        : base() { }
}
