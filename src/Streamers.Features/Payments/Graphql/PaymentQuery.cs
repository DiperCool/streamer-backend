using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Payments.Dtos;
using Streamers.Features.Payments.Features.GetPaymentMethods;

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
}

