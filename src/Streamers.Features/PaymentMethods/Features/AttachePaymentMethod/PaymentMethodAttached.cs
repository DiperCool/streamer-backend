using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.PaymentMethods.Models;
using Streamers.Features.Shared.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Exceptions;

namespace Streamers.Features.PaymentMethods.Features.AttachePaymentMethod;

public record PaymentMethodAttachedResponse;

[Transactional]
public record PaymentMethodAttached(
    string StripePaymentMethodId,
    string CustomerId,
    string CardBrand,
    string CardLast4,
    long CardExpMonth,
    long CardExpYear
) : IRequest<PaymentMethodAttachedResponse>;

public class PaymentMethodAttachedHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<PaymentMethodAttached, PaymentMethodAttachedResponse>
{
    public async Task<PaymentMethodAttachedResponse> Handle(
        PaymentMethodAttached request,
        CancellationToken cancellationToken
    )
    {
        var streamer = await streamerDbContext.Streamers.FirstOrDefaultAsync(
            x => x.Customer.StripeCustomerId == request.CustomerId,
            cancellationToken: cancellationToken
        );
        if (streamer == null)
        {
            throw new StreamerNotFoundException(request.CustomerId);
        }
        var paymentMethod = new PaymentMethod(
            request.StripePaymentMethodId,
            streamer.Id,
            request.CardBrand,
            request.CardLast4,
            request.CardExpMonth,
            request.CardExpYear
        );

        streamerDbContext.PaymentMethods.Add(paymentMethod);
        await streamerDbContext.SaveChangesAsync(cancellationToken);

        return new PaymentMethodAttachedResponse();
    }
}
