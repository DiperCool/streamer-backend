using Shared.Abstractions.Domain;
using Streamers.Features.Analytics.Enums;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Analytics.Models;

public class AnalyticsItem : Entity
{
    protected AnalyticsItem() { }

    public AnalyticsItem(
        double value,
        DateTime createdAt,
        AnalyticsItemType type,
        string streamerId
    )
    {
        StreamerId = streamerId;
        Value = value;
        CreatedAt = createdAt;
        Type = type;
    }

    public double Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public AnalyticsItemType Type { get; set; }
    public string StreamerId { get; set; }
    public Streamer Streamer { get; set; }
}
