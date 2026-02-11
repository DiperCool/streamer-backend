using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.PaymentMethods.Features.CreateSetupIntent;
using Streamers.Features.PaymentMethods.Features.MakePaymentMethodDefault;
using Streamers.Features.PaymentMethods.Features.RemovePaymentMethod;

namespace Streamers.Features.PaymentMethods.Graphql;

[MutationType]
[Authorize]
public static class PaymentMethodMutation
{
    public static async Task<CreateSetupIntentResponse> CreateSetupIntent(
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        return await mediator.Send(new CreateSetupIntent(), cancellationToken);
    }

    public static async Task<RemovePaymentMethodResponse> RemovePaymentMethod(
        Guid paymentMethodId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        return await mediator.Send(new RemovePaymentMethod(paymentMethodId), cancellationToken);
    }

    public static async Task<MakePaymentMethodDefaultResponse> MakePaymentMethodDefault(
        Guid paymentMethodId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        return await mediator.Send(
            new MakePaymentMethodDefault(paymentMethodId),
            cancellationToken
        );
    }
}
