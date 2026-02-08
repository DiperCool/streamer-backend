using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Payments.Dtos;
using Streamers.Features.Payments.Features.GetPaymentMethods;
using Streamers.Features.Payments.Features.GetPartner;

namespace Streamers.Features.Payments.Graphql;

[QueryType]
[Authorize]
public static class PaymentQuery
{
    public static async Task<List<PaymentMethodDto>> GetPaymentMethods(
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var response = await mediator.Send(new GetPaymentMethods(), cancellationToken);
        return response.PaymentMethods;
    }

    public static async Task<PartnerDto> GetPartner(
        string streamerId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        return await mediator.Send(new GetPartner(streamerId), cancellationToken);
    }
}

