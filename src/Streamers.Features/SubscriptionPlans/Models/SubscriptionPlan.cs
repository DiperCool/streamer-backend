using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.SubscriptionPlans.Models;

public class SubscriptionPlan : Entity<Guid>
{
    public string StreamerId { get; private set; }
    public Streamer Streamer { get; private set; }
    public string StripeProductId { get; private set; }
    public string StripePriceId { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private SubscriptionPlan() { }

    public SubscriptionPlan(string streamerId, string stripeProductId, string stripePriceId, string name, decimal price)
    {
        StreamerId = streamerId;
        StripeProductId = stripeProductId;
        StripePriceId = stripePriceId;
        Name = name;
        Price = price;
        CreatedAt = DateTime.UtcNow;
    }
}
