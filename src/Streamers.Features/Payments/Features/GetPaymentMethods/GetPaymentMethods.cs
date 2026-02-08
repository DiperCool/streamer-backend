using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Payments.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Payments.Features.GetPaymentMethods;

public record GetPaymentMethods() : IRequest<GetPaymentMethodsResponse>;

public record GetPaymentMethodsResponse(List<PaymentMethodDto> PaymentMethods);

public class GetPaymentMethodsHandler(ICurrentUser currentUser, StreamerDbContext streamerDbContext)
    : IRequestHandler<GetPaymentMethods, GetPaymentMethodsResponse>
{
    public async Task<GetPaymentMethodsResponse> Handle(
        GetPaymentMethods request,
        CancellationToken cancellationToken
    )
    {
        var paymentMethods = await streamerDbContext
            .PaymentMethods.Where(x => x.StreamerId == currentUser.UserId)
            .Select(x => new PaymentMethodDto
            {
                Id = x.Id,
                CardBrand = x.CardBrand,
                CardLast4 = x.CardLast4,
                CardExpMonth = x.CardExpMonth,
                CardExpYear = x.CardExpYear,
                IsDefault = x.IsDefault,
            })
            .ToListAsync(cancellationToken);

        return new GetPaymentMethodsResponse(paymentMethods);
    }
}
