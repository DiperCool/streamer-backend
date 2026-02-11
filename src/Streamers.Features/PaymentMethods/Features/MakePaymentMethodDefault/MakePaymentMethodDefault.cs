using System.Reactive;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Shared.Stripe;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers;

namespace Streamers.Features.PaymentMethods.Features.MakePaymentMethodDefault;

public record MakePaymentMethodDefaultResponse(Guid Id);

public record MakePaymentMethodDefault(Guid PaymentMethodId)
    : IRequest<MakePaymentMethodDefaultResponse>;

public class MakePaymentMethodDefaultHandler(
    ICurrentUser currentUser,
    StreamerDbContext streamerDbContext
) : IRequestHandler<MakePaymentMethodDefault, MakePaymentMethodDefaultResponse>
{
    public async Task<MakePaymentMethodDefaultResponse> Handle(
        MakePaymentMethodDefault request,
        CancellationToken cancellationToken
    )
    {
        var paymentMethodToSetDefault = await streamerDbContext
            .PaymentMethods.Where(x =>
                x.Id == request.PaymentMethodId && x.StreamerId == currentUser.UserId
            )
            .Include(x => x.Streamer)
            .ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(cancellationToken);

        if (paymentMethodToSetDefault == null)
        {
            throw new InvalidOperationException("Payment method not found");
        }

        await streamerDbContext
            .PaymentMethods.Where(x =>
                x.StreamerId == currentUser.UserId && x.Id != request.PaymentMethodId
            )
            .ExecuteUpdateAsync(
                s => s.SetProperty(b => b.IsDefault, b => false),
                cancellationToken
            );

        paymentMethodToSetDefault.MakeDefault();
        streamerDbContext.PaymentMethods.Update(paymentMethodToSetDefault);
        await streamerDbContext.SaveChangesAsync(cancellationToken);

        return new MakePaymentMethodDefaultResponse(request.PaymentMethodId);
    }
}
