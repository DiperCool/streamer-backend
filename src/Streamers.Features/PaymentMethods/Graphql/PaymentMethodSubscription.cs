using System.Runtime.CompilerServices;
using System.Security.Claims;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using streamer.ServiceDefaults;
using Streamers.Features.PaymentMethods.Dtos;

namespace Streamers.Features.PaymentMethods.Graphql;

[SubscriptionType]
public static partial class PaymentMethodSubscription
{
    public static async IAsyncEnumerable<PaymentMethodDto> SubscribePaymentMethodCreated(
        [Service] ITopicEventReceiver receiver,
        [GlobalState(nameof(ClaimsPrincipal))] ClaimsPrincipal user,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            yield break;
        }
        string topic = $"{nameof(PaymentMethodCreated)}-{user.GetUserId()}";

        ISourceStream<PaymentMethodDto> stream = await receiver.SubscribeAsync<PaymentMethodDto>(
            topic,
            cancellationToken
        );

        await foreach (
            PaymentMethodDto evt in stream.ReadEventsAsync().WithCancellation(cancellationToken)
        )
        {
            yield return evt;
        }
    }

    [Subscribe(With = nameof(SubscribePaymentMethodCreated))]
    public static PaymentMethodDto PaymentMethodCreated(
        [EventMessage] PaymentMethodDto paymentMethod
    ) => paymentMethod;

    public static async IAsyncEnumerable<PaymentMethodDeletedDto> SubscribePaymentMethodDeleted(
        [Service] ITopicEventReceiver receiver,
        [GlobalState(nameof(ClaimsPrincipal))] ClaimsPrincipal user,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            yield break;
        }
        string topic = $"{nameof(PaymentMethodDeleted)}-{user.GetUserId()}";

        ISourceStream<PaymentMethodDeletedDto> stream = await receiver.SubscribeAsync<PaymentMethodDeletedDto>(
            topic,
            cancellationToken
        );

        await foreach (
            PaymentMethodDeletedDto evt in stream.ReadEventsAsync().WithCancellation(cancellationToken)
        )
        {
            yield return evt;
        }
    }

    [Subscribe(With = nameof(SubscribePaymentMethodDeleted))]
    public static PaymentMethodDeletedDto PaymentMethodDeleted(
        [EventMessage] PaymentMethodDeletedDto paymentMethodDeleted
    ) => paymentMethodDeleted;
}
