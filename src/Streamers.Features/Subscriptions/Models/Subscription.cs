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
    public string Title { get; private set; }

    private Subscription() { }

    public Subscription(
        Guid id,
        string userId,
        string streamerId,
        string stripeSubscriptionId,
        SubscriptionStatus status,
        DateTime createdAt,
        string title
    )
    {
        Id = id;
        UserId = userId;
        StreamerId = streamerId;
        StripeSubscriptionId = stripeSubscriptionId;
        Status = status;
        CurrentPeriodEnd = DateTime.MinValue;
        CreatedAt = createdAt;
        Title = title;
    }

    public void SetStatus(SubscriptionStatus status)
    {
        Status = status;
    }

    public void SetCurrentPeriodEnd(DateTime currentPeriodEnd)
    {
        CurrentPeriodEnd = currentPeriodEnd;
    }
}
