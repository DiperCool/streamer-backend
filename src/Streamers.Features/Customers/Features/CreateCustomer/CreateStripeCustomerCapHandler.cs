using DotNetCore.CAP;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Streamers.Dtos;

namespace Streamers.Features.Customers.Features.CreateCustomer;

public class CreateStripeCustomerCapHandler(IMediator mediator) : ICapSubscribe
{
    [CapSubscribe("streamer.created")]
    public async Task Handle(StreamerEvent domainEvent, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new CreateStripeCustomerCommand(domainEvent.Id, domainEvent.Email),
            cancellationToken
        );
    }
}
