namespace Streamers.Features.Transactions.Models;

public enum TransactionType
{
    Subscription,
}

public enum TransactionStatus
{
    Pending,
    Succeeded,
    Failed,
    Refunded,
}
