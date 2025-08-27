using System.Data;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Shared.Cqrs.Behaviours;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly StreamerDbContext _dbContext;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly ICapPublisher _publisher;

    public TransactionBehavior(
        StreamerDbContext dbContext,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger,
        ICapPublisher publisher
    )
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var type = request.GetType();
        var typeName = type.Name;
        var transactionalAttr =
            type.GetCustomAttributes(typeof(TransactionalAttribute), false).FirstOrDefault()
            as TransactionalAttribute;
        if (transactionalAttr == null)
        {
            return await next();
        }

        if (_dbContext.Database.CurrentTransaction != null)
        {
            return await next();
        }

        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(
                transactionalAttr.IsolationLevel,
                _publisher,
                true,
                cancellationToken: cancellationToken
            );

            try
            {
                _logger.LogInformation(
                    "Begin transaction {TransactionId} with isolation {Isolation} for {CommandName}",
                    transaction.TransactionId,
                    transactionalAttr.IsolationLevel,
                    typeName
                );

                var response = await next();

                _logger.LogInformation(
                    "Committed transaction {TransactionId} for {CommandName}",
                    transaction.TransactionId,
                    typeName
                );

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling transaction for {CommandName}", typeName);
                throw;
            }
        });
    }
}
