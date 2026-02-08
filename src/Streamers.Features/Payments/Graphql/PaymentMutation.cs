using System.Reactive;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Payments.Features.BecomePartner;
using Streamers.Features.Payments.Features.CreateSetupIntent;
using Streamers.Features.Payments.Features.GenerateOnboardingLink;
using Streamers.Features.Payments.Features.MakePaymentMethodDefault;
using Streamers.Features.Payments.Features.RemovePaymentMethod;

namespace Streamers.Features.Payments.Graphql;

[MutationType]
[Authorize]
public static class PaymentMutation
{
    public static async Task<BecomePartnerResponse> BecomePartner(
        string streamerId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        return await mediator.Send(new BecomePartner(streamerId), cancellationToken);
    }

    public static async Task<OnboardingLinkResponse> GenerateOnboardingLink(
        string streamerId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        return await mediator.Send(new GenerateOnboardingLinkQuery(streamerId), cancellationToken);
    }

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
