using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Partners.Features.BecomePartner;
using Streamers.Features.Partners.Features.GenerateOnboardingLink;

namespace Streamers.Features.Partners.Graphql;

[MutationType]
[Authorize]
public static class PartnerMutation
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
}
