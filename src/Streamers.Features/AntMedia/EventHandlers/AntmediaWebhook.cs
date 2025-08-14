using System.Text.Json;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Cqrs;
using Streamers.Features.AntMedia.Services;
using Streamers.Features.Streamers.EventHandlers;

namespace Streamers.Features.AntMedia.EventHandlers;

public interface IAntmediaWebhook
{
    Task HandleWebhook(JsonElement evt);
}

public class AntmediaWebhook(ILogger<AntmediaWebhook> logger, IAntMediaWebhookHandlerFabric fabric)
    : IAntmediaWebhook,
        ICapSubscribe
{
    [CapSubscribe("mediamtx")]
    public async Task HandleWebhook(JsonElement evt)
    {
        var payload = evt.Deserialize<AntMediaWebhookPayload>();

        if (payload == null)
        {
            return;
        }
        logger.LogInformation(
            $"Received webhook for stream: {payload.StreamName} (ID: {payload.Id})"
        );
        var handler = fabric.Create(payload.Action);
        await handler.HandleAsync(payload);
    }
}
