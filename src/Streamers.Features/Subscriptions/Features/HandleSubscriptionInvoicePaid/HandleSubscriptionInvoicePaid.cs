using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Subscriptions.Models;

namespace Streamers.Features.Subscriptions.Features.HandleSubscriptionInvoicePaid;

public record HandleSubscriptionInvoicePaid(string StripeSubscriptionId, DateTime CurrentPeriodEnd)
    : IRequest<bool>;

public class HandleSubscriptionInvoicePaidHandler(StreamerDbContext context)
    : IRequestHandler<HandleSubscriptionInvoicePaid, bool>
{
    public async Task<bool> Handle(
        HandleSubscriptionInvoicePaid request,
        CancellationToken cancellationToken
    )
    {
        var subscription = await context
            .Subscriptions.IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                s => s.StripeSubscriptionId == request.StripeSubscriptionId,
                cancellationToken
            );

        if (subscription is null)
        {
            return false;
        }

        if (subscription.Status == SubscriptionStatus.Incomplete)
        {
            subscription.MakeCurrentAndSucceedPayment();
        }
        else
        {
            subscription.SucceedPayment();
        }
        subscription.SetCurrentPeriodEnd(request.CurrentPeriodEnd);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
