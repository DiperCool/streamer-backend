using Shared.Abstractions.Cqrs;
using Streamers.Features.Streams.Features.CreateStream;

namespace Streamers.Features.AntMedia.Services;

public class LiveStreamStartedHandler(IMediator mediator) : IAntMediaWebhookHandler
{
    public async Task HandleAsync(AntMediaWebhookPayload payload)
    {
        await mediator.Send(new CreateStream(payload.StreamName ?? "", payload.Id));
    }
}
