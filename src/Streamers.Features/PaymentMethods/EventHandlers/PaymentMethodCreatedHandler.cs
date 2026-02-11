using HotChocolate.Subscriptions;
using Shared.Abstractions.Domain;
using Streamers.Features.PaymentMethods.Dtos;
using Streamers.Features.PaymentMethods.Models;

namespace Streamers.Features.PaymentMethods.EventHandlers;

public class PaymentMethodCreatedHandler(ITopicEventSender sender)
    : IDomainEventHandler<PaymentMethodCreated>
{
    public async Task Handle(PaymentMethodCreated notification, CancellationToken cancellationToken)
    {
        var dto = new PaymentMethodDto
        {
            Id = notification.PaymentMethod.Id,
            CardBrand = notification.PaymentMethod.CardBrand,
            CardLast4 = notification.PaymentMethod.CardLast4,
            CardExpMonth = notification.PaymentMethod.CardExpMonth,
            CardExpYear = notification.PaymentMethod.CardExpYear,
            IsDefault = notification.PaymentMethod.IsDefault,
        };

        await sender.SendAsync(
            $"{nameof(PaymentMethodCreated)}-{notification.PaymentMethod.StreamerId}",
            dto,
            cancellationToken
        );
    }
}
