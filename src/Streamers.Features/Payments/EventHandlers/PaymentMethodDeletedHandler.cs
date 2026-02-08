using HotChocolate.Subscriptions;
using Shared.Abstractions.Domain;
using Streamers.Features.Payments.Dtos;
using Streamers.Features.Payments.Models;

namespace Streamers.Features.Payments.EventHandlers;

public class PaymentMethodDeletedHandler(ITopicEventSender sender)
    : IDomainEventHandler<PaymentMethodDeleted>
{
    public async Task Handle(PaymentMethodDeleted notification, CancellationToken cancellationToken)
    {
        var dto = new PaymentMethodDeletedDto
        {
            PaymentMethodId = notification.PaymentMethodId,
            StreamerId = notification.StreamerId,
        };
        await sender.SendAsync(
            $"{nameof(PaymentMethodDeleted)}-{notification.StreamerId}",
            dto,
            cancellationToken
        );
    }
}
