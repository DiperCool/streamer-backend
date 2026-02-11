using System.Reactive;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers;

namespace Streamers.Features.PaymentMethods.Features.RemovePaymentMethod;

public record RemovePaymentMethodResponse(Guid Id);

public record RemovePaymentMethod(Guid PaymentMethodId) : IRequest<RemovePaymentMethodResponse>;

public class RemovePaymentMethodHandler(
    ICurrentUser currentUser,
    StreamerDbContext streamerDbContext,
    IStripeService stripeService
) : IRequestHandler<RemovePaymentMethod, RemovePaymentMethodResponse>
{
    public async Task<RemovePaymentMethodResponse> Handle(
        RemovePaymentMethod request,
        CancellationToken cancellationToken
    )
    {
        var paymentMethod = await streamerDbContext
            .PaymentMethods.Where(x =>
                x.Id == request.PaymentMethodId && x.StreamerId == currentUser.UserId
            )
            .Include(x => x.Streamer)
            .ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(cancellationToken);

        if (paymentMethod == null)
        {
            throw new InvalidOperationException("Payment Method not found");
        }

        await stripeService.DetachPaymentMethodAsync(
            paymentMethod.Streamer.Customer.StripeCustomerId
                ?? throw new InvalidOperationException("Stripe Error"),
            paymentMethod.StripePaymentMethodId,
            cancellationToken
        );

        return new RemovePaymentMethodResponse(request.PaymentMethodId);
    }
}
