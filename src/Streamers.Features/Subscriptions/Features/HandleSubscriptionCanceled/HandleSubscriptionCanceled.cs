using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Subscriptions.Models;

namespace Streamers.Features.Subscriptions.Features.HandleSubscriptionCanceled;

public record HandleSubscriptionCanceled(string StripeSubscriptionId) : IRequest<bool>;

public class HandleSubscriptionCanceledHandler(StreamerDbContext context)
    : IRequestHandler<HandleSubscriptionCanceled, bool>
{
    public async Task<bool> Handle(HandleSubscriptionCanceled request, CancellationToken cancellationToken)
    {
        var subscription = await context.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == request.StripeSubscriptionId, cancellationToken);

        if (subscription is null)
        {
            return false;
        }

        subscription.SetStatus(SubscriptionStatus.Canceled);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
