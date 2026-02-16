using Shared.Abstractions.Domain;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Transactions.Models;

public record TransactionUpdated(Transaction Transaction) : IDomainEvent;

public class Transaction : Entity<Guid>
{
    public Transaction(Guid id, string userId, Streamer user, string streamerId, Streamer streamer, TransactionType transactionType, decimal grossAmount, decimal platformFee, decimal streamerNet, TransactionStatus status, DateTime createdAt, string? stripeInvoiceUrl)
    {
        Id = id;
        UserId = userId;
        User = user;
        StreamerId = streamerId;
        Streamer = streamer;
        TransactionType = transactionType;
        GrossAmount = grossAmount;
        PlatformFee = platformFee;
        StreamerNet = streamerNet;
        Status = status;
        CreatedAt = createdAt;
        StripeInvoiceUrl = stripeInvoiceUrl;
    }

    // For EF Core
    private Transaction() { } 

    public string UserId { get; private set; } = default!;
    public Streamer User { get; private set; } = default!; 

    public string StreamerId { get; private set; } = default!;
    public Streamer Streamer { get; private set; } = default!; 

    public TransactionType TransactionType { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal PlatformFee { get; private set; }
    public decimal StreamerNet { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string? StripeInvoiceUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public void UpdateStatus(TransactionStatus newStatus)
    {
        Status = newStatus;
        Raise(new TransactionUpdated(this));
    }

    public void SetStripeInvoiceUrl(string url)
    {
        StripeInvoiceUrl = url;
    }
}
