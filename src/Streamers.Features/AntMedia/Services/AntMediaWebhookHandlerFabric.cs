using Microsoft.Extensions.DependencyInjection;

namespace Streamers.Features.AntMedia.Services;

public class AntMediaWebhookHandlerFabric(IServiceProvider provider) : IAntMediaWebhookHandlerFabric
{
    public IAntMediaWebhookHandler Create(string eventType)
    {
        return eventType switch
        {
            "liveStreamStarted" => provider.GetRequiredService<LiveStreamStartedHandler>(),
            "liveStreamEnded" => provider.GetRequiredService<LiveStreamEndedHandler>(),
            "streamRead" => provider.GetRequiredService<AddReaderHandler>(),
            "streamUnread" => provider.GetRequiredService<RemoveReaderHandler>(),
            _ => throw new ArgumentException($"Unknown event type: {eventType}", nameof(eventType)),
        };
    }
}
