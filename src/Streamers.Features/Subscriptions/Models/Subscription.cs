using System.ComponentModel.DataAnnotations.Schema;
using Shared.Abstractions.Domain;

namespace Streamers.Features.Subscriptions.Models;

public enum SubscriptionStatus
{
    Incomplete,
    Active,
    Canceled,
    PastDue,
}

public class Subscription : Entity<Guid>
{
    public string UserId { get; private set; }
    public string StreamerId { get; private set; }
    public string StripeSubscriptionId { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime CurrentPeriodEnd { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Subscription() { }

    public Subscription(
        Guid id,
        string userId,
        string streamerId,
        string stripeSubscriptionId,
        SubscriptionStatus status,
        DateTime createdAt
    )
    {
        Id = id;
        UserId = userId;
        StreamerId = streamerId;
        StripeSubscriptionId = stripeSubscriptionId;
        Status = status;
        CurrentPeriodEnd = DateTime.MinValue;
        CreatedAt = createdAt;
    }
}
