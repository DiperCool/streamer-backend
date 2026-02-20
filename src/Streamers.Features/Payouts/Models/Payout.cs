using Shared.Abstractions.Domain;
using Streamers.Features.Payouts.Enums;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Payouts.Models;

public class Payout : Entity<Guid>
{
    public string StreamerId { get; private set; } = default!;
    public Streamer Streamer { get; private set; } = default!;
    public string StripePayoutId { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = default!;
    public PayoutStatus Status { get; private set; }
    public DateTime ArrivalDate { get; private set; }
    public string? FailureMessage { get; private set; }
    public decimal ApplicationFee { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Payout() { }

    public Payout(
        Guid id,
        string streamerId,
        string stripePayoutId,
        decimal amount,
        string currency,
        DateTime arrivalDate,
        DateTime createdAt,
        decimal applicationFee = 0,
        string? failureMessage = null
    )
    {
        Id = id;
        StreamerId = streamerId;
        StripePayoutId = stripePayoutId;
        Amount = amount;
        Currency = currency;
        Status = PayoutStatus.Pending;
        ArrivalDate = arrivalDate;
        CreatedAt = createdAt;
        ApplicationFee = applicationFee;
        FailureMessage = failureMessage;
    }

    public void MakePaid()
    {
        Status = PayoutStatus.Paid;
        FailureMessage = null;
    }

    public void MakeFailed(string failureMessage)
    {
        Status = PayoutStatus.Failed;
        FailureMessage = failureMessage;
    }
}
