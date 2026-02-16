using Streamers.Features.Transactions.Models;

namespace Streamers.Features.Transactions.Dtos;

public class TransactionDto
{
    public required Guid Id { get; set; }
    public required string UserId { get; set; }
    public required string StreamerId { get; set; }
    public required TransactionType TransactionType { get; set; }
    public required decimal GrossAmount { get; set; }
    public required decimal PlatformFee { get; set; }
    public required decimal StreamerNet { get; set; }
    public required TransactionStatus Status { get; set; }
    public string? StripeInvoiceUrl { get; set; }
    public required DateTime CreatedAt { get; set; }
}
