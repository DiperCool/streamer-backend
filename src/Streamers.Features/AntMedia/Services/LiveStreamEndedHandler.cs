using Shared.Abstractions.Cqrs;
using Streamers.Features.Streams.Features.EndStream;

namespace Streamers.Features.AntMedia.Services;

public class LiveStreamEndedHandler(IMediator mediator) : IAntMediaWebhookHandler
{
    public async Task HandleAsync(AntMediaWebhookPayload payload)
    {
        await mediator.Send(new EndStream(payload.Id));
    }
}
