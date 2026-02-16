using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Transactions.Models;

namespace Streamers.Features.Transactions.Features.CreateTransaction;

public record CreateTransaction(
    string UserId,
    string StreamerId,
    TransactionType TransactionType,
    decimal GrossAmount,
    TransactionStatus Status,
    string? StripeInvoiceUrl
) : IRequest<CreateTransactionResponse>;

public record CreateTransactionResponse(Guid Id);

public class CreateTransactionHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<CreateTransaction, CreateTransactionResponse>
{
    public async Task<CreateTransactionResponse> Handle(
        CreateTransaction request,
        CancellationToken cancellationToken
    )
    {
        var user = await streamerDbContext.Streamers.FirstOrDefaultAsync(
            x => x.Id == request.UserId,
            cancellationToken
        );
        var streamer = await streamerDbContext.Streamers.FirstOrDefaultAsync(
            x => x.Id == request.StreamerId,
            cancellationToken
        );

        if (user == null)
        {
            throw new Exception($"User with ID {request.UserId} not found.");
        }

        if (streamer == null)
        {
            throw new Exception($"Streamer with ID {request.StreamerId} not found.");
        }

        const decimal applicationFeePercent = 5;

        var platformFee = request.GrossAmount * (applicationFeePercent / 100m);
        var streamerNet = request.GrossAmount - platformFee;

        var transaction = new Transaction(
            Guid.NewGuid(),
            request.UserId,
            user,
            request.StreamerId,
            streamer,
            request.TransactionType,
            request.GrossAmount,
            platformFee,
            streamerNet,
            request.Status,
            DateTime.UtcNow,
            request.StripeInvoiceUrl
        );

        await streamerDbContext.Transactions.AddAsync(transaction, cancellationToken);
        await streamerDbContext.SaveChangesAsync(cancellationToken);

        return new CreateTransactionResponse(transaction.Id);
    }
}
