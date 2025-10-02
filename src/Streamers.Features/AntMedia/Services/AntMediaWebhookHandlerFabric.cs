using Microsoft.Extensions.DependencyInjection;

namespace Streamers.Features.AntMedia.Services;

public class AntMediaWebhookHandlerFabric(IServiceProvider provider) : IAntMediaWebhookHandlerFabric
{
    public IAntMediaWebhookHandler? Create(string eventType)
    {
        return eventType switch
        {
            "liveStreamStarted" => provider.GetRequiredService<LiveStreamStartedHandler>(),
            "liveStreamEnded" => provider.GetRequiredService<LiveStreamEndedHandler>(),
            _ => null,
        };
    }
}
