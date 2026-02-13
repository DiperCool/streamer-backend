using GreenDonut;
using HotChocolate;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Subscriptions.Dtos;
using Streamers.Features.Subscriptions.Features.GetSubscriptionsByIds;

namespace Streamers.Features.Subscriptions.Graphql;

public static partial class SubscriptionDataLoader
{
    [DataLoader]
    public static async Task<IDictionary<Guid, SubscriptionDto>> GetSubscriptionsByIdAsync(
        IReadOnlyList<Guid> ids,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new GetSubscriptionsByIds(ids), cancellationToken);
        return response.Subscriptions;
    }
}
