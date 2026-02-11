using HotChocolate.Subscriptions;
using Shared.Abstractions.Domain;
using Streamers.Features.PaymentMethods.Dtos;
using Streamers.Features.PaymentMethods.Models;

namespace Streamers.Features.PaymentMethods.EventHandlers;

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
