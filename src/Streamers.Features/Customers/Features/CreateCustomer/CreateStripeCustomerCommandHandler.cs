using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Customers.Features.CreateCustomer;

public record CreateStripeCustomerCommandResponse;

public record CreateStripeCustomerCommand(string StreamerId, string Email)
    : IRequest<CreateStripeCustomerCommandResponse>;

public class CreateStripeCustomerCommandHandler(
    IStripeService stripeService,
    StreamerDbContext streamerDbContext,
    ILogger<CreateStripeCustomerCommandHandler> logger
) : IRequestHandler<CreateStripeCustomerCommand, CreateStripeCustomerCommandResponse>
{
    public async Task<CreateStripeCustomerCommandResponse> Handle(
        CreateStripeCustomerCommand request,
        CancellationToken cancellationToken
    )
    {
        var streamer = await streamerDbContext
            .Streamers.IgnoreQueryFilters()
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.StreamerId, cancellationToken);
        if (streamer is null)
        {
            throw new InvalidOperationException($"Streamer {request.StreamerId} not found");
        }

        try
        {
            var stripeId = await stripeService.CreateCustomerAsync(
                request.StreamerId,
                request.Email,
                cancellationToken
            );
            streamer.Customer.MarkAsSuccess(stripeId);
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Failed to create stripe customer for streamer {StreamerId}",
                request.StreamerId
            );
            streamer.Customer.MarkAsFailed();
        }
        finally
        {
            await streamerDbContext.SaveChangesAsync(cancellationToken);
        }

        return new CreateStripeCustomerCommandResponse();
    }
}
