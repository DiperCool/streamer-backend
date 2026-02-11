using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Subscriptions.Models;

namespace Streamers.Features.Subscriptions.Features.CreateSubscription;

public record SubscriptionDto(
    Guid Id,
    string UserId,
    string StreamerId,
    string StripeSubscriptionId,
    SubscriptionStatus Status,
    DateTime CurrentPeriodEnd,
    DateTime CreatedAt
);

public record CreateSubscription(
    string UserId,
    string StreamerId,
    string StripeSubscriptionId,
    DateTime CurrentPeriodEnd
) : IRequest<CreateSubscriptionResponse>;

public record CreateSubscriptionResponse(SubscriptionDto Subscription);

public class CreateSubscriptionHandler(StreamerDbContext dbContext)
    : IRequestHandler<CreateSubscription, CreateSubscriptionResponse>
{
    public async Task<CreateSubscriptionResponse> Handle(
        CreateSubscription request,
        CancellationToken cancellationToken
    )
    {
        var subscription = new Subscription(
            Guid.NewGuid(),
            request.UserId,
            request.StreamerId,
            request.StripeSubscriptionId,
            SubscriptionStatus.Active,
            request.CurrentPeriodEnd,
            DateTime.UtcNow
        );

        dbContext.Subscriptions.Add(subscription);
        await dbContext.SaveChangesAsync(cancellationToken);

        var subscriptionDto = new SubscriptionDto(
            subscription.Id,
            subscription.UserId,
            subscription.StreamerId,
            subscription.StripeSubscriptionId,
            subscription.Status,
            subscription.CurrentPeriodEnd,
            subscription.CreatedAt
        );

        return new CreateSubscriptionResponse(subscriptionDto);
    }
}
