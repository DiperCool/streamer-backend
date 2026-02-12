using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Subscriptions.Features.CreateSubscription;

namespace Streamers.Features.Subscriptions.Graphql;

[MutationType]
public static partial class SubscriptionMutation
{
    [Authorize]
    public static async Task<CreateSubscriptionResponse> CreateSubscription(
        Guid subscriptionPlanId,
        Guid paymentMethodId,
        [Service] IMediator mediator
    )
    {
        var response = await mediator.Send(
            new CreateSubscription(subscriptionPlanId, paymentMethodId)
        );
        return response;
    }
}
