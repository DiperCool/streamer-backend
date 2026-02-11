using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Partners.Dtos;
using Streamers.Features.Partners.Features.GetPartner;

namespace Streamers.Features.Partners.Graphql;

[QueryType]
[Authorize]
public static class PartnerQuery
{
    public static async Task<PartnerDto> GetPartner(
        string streamerId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        return await mediator.Send(new GetPartner(streamerId), cancellationToken);
    }
}
