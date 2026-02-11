using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.SubscriptionPlans.Dtos;
using Streamers.Features.SubscriptionPlans.Features.GetSubscriptionPlansByStreamerId;

namespace Streamers.Features.SubscriptionPlans.Graphql;

[QueryType]
public static partial class SubscriptionPlanQuery
{
    [Authorize]
    public static async Task<IEnumerable<SubscriptionPlanDto>> GetSubscriptionPlansByStreamerId(
        string streamerId,
        [Service] IMediator mediator
    )
    {
        var response = await mediator.Send(new GetSubscriptionPlansByStreamerId(streamerId));
        return response;
    }
}
