using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.SubscriptionPlans.Features.CreatePaymentIntent;

namespace Streamers.Features.SubscriptionPlans.Graphql;

[MutationType]
public static partial class SubscriptionPlanMutation
{
    [Authorize]
    public static async Task<CreatePaymentIntentResponse> CreatePaymentIntent(
        Guid subscriptionPlanId,
        string paymentMethodId,
        [Service] IMediator mediator
    )
    {
        var response = await mediator.Send(
            new CreatePaymentIntent(subscriptionPlanId, paymentMethodId)
        );
        return response;
    }
}
