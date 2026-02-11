using Shared.Abstractions.Domain;
using Streamers.Features.Customers.Enums;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Customers.Models;

public class Customer : Entity<Guid>
{
    public Streamer Streamer { get; private set; }
    public string StreamerId { get; private set; }
    public string? StripeCustomerId { get; private set; }
    public StripeCustomerCreationStatus StripeCustomerCreationStatus { get; private set; } =
        StripeCustomerCreationStatus.Pending;

    public void MarkAsSuccess(string stripeCustomerId)
    {
        StripeCustomerId = stripeCustomerId;
        StripeCustomerCreationStatus = StripeCustomerCreationStatus.Success;
    }

    public void MarkAsFailed()
    {
        StripeCustomerCreationStatus = StripeCustomerCreationStatus.Failed;
    }
}
