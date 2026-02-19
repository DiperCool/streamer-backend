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
    public string Status { get; private set; } = default!;
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
        string status,
        DateTime arrivalDate,
        DateTime createdAt,
        decimal applicationFee = 0,
        string? failureMessage = null)
    {
        Id = id;
        StreamerId = streamerId;
        StripePayoutId = stripePayoutId;
        Amount = amount;
        Currency = currency;
        Status = status;
        ArrivalDate = arrivalDate;
        CreatedAt = createdAt;
        ApplicationFee = applicationFee;
        FailureMessage = failureMessage;
    }
    
    public void MakePaid()
    {
        Status = PayoutStatus.PayoutPaid.ToString();
        FailureMessage = null;
    }

    public void MakeFailed(string failureMessage)
    {
        Status = PayoutStatus.PayoutFailed.ToString();
        FailureMessage = failureMessage;
    }
}
