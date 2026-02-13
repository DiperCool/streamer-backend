using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Subscriptions.Models;

namespace Streamers.Features.Subscriptions.Features.HandleSubscriptionPastDue;

public record HandleSubscriptionPastDue(string StripeSubscriptionId) : IRequest<bool>;

public class HandleSubscriptionPastDueHandler(StreamerDbContext context)
    : IRequestHandler<HandleSubscriptionPastDue, bool>
{
    public async Task<bool> Handle(HandleSubscriptionPastDue request, CancellationToken cancellationToken)
    {
        var subscription = await context.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == request.StripeSubscriptionId, cancellationToken);

        if (subscription is null)
        {
            return false;
        }

        subscription.SetStatus(SubscriptionStatus.PastDue);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
