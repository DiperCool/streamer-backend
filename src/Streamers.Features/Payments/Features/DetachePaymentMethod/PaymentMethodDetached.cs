using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Cqrs;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Payments.Features.DetachePaymentMethod;

public record PaymentMethodDetachedResponse();

[Transactional]
public record PaymentMethodDetached(string StripePaymentMethodId)
    : IRequest<PaymentMethodDetachedResponse>;

public class PaymentMethodDetachedHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<PaymentMethodDetached, PaymentMethodDetachedResponse>
{
    public async Task<PaymentMethodDetachedResponse> Handle(
        PaymentMethodDetached request,
        CancellationToken cancellationToken
    )
    {
        var paymentMethod = await streamerDbContext
            .PaymentMethods.Where(x => x.StripePaymentMethodId == request.StripePaymentMethodId)
            .FirstOrDefaultAsync(cancellationToken);

        if (paymentMethod == null)
        {
            throw new InvalidOperationException("Payment method not found");
        }

        paymentMethod.Delete();
        streamerDbContext.PaymentMethods.Remove(paymentMethod);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new PaymentMethodDetachedResponse();
    }
}
