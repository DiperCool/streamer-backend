using System.Text.Json;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Cqrs;
using Streamers.Features.AntMedia.Services;
using Streamers.Features.Vods.Features.VodFinishProcess;

namespace Streamers.Features.Vods.EventHandler;

public interface IVodFinishedHandler
{
    Task HandleWebhook(JsonElement evt);
}

public class VodFinishedHandler(ILogger<VodFinishedHandler> logger, IMediator mediator)
    : IVodFinishedHandler,
        ICapSubscribe
{
    [CapSubscribe("vod-process-finished")]
    public async Task HandleWebhook(JsonElement evt)
    {
        var payload = evt.Deserialize<VodFinishProcess>();

        if (payload == null)
        {
            return;
        }
        await mediator.Send(payload);
        logger.LogInformation($"Vod Id = {payload.VodId} Finished");
    }
}
