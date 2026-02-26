using Streamers.Features.Streamers.Models;

namespace Streamers.Features.ModerationActivities.Models;

public class StreamCategoryAction : ModeratorActionType
{
    public string NewCategory { get; protected set; } = default!;

    public StreamCategoryAction(string moderatorId, string streamerId, string newCategory)
        : base("StreamCategory", moderatorId, streamerId)
    {
        NewCategory = newCategory;
    }

    private StreamCategoryAction()
        : base() { }
}
