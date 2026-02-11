using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.PaymentMethods.Dtos;
using Streamers.Features.PaymentMethods.Features.GetPaymentMethods;

namespace Streamers.Features.PaymentMethods.Graphql;

[QueryType]
[Authorize]
public static class PaymentMethodQuery
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
