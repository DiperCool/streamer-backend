using System.Text.Json;
using DotNetCore.CAP;
using Shared.Abstractions.Cqrs;

namespace Streamers.Features.Streamers.Features.UpdateStreamPreview.EventHandler;

public class StreamPreviewUpdated(IMediator mediator) : ICapSubscribe
{
    [CapSubscribe("stream-preview-updated")]
    public async Task HandleStreamPreviewUpdated(JsonElement evt)
    {
        var command = evt.Deserialize<UpdateStreamPreview>();
        if (command == null)
        {
            return;
        }
        await mediator.Send(command);
    }
}
