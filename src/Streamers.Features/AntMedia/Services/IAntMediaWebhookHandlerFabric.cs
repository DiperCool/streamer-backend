namespace Streamers.Features.AntMedia.Services;

public interface IAntMediaWebhookHandlerFabric
{
    IAntMediaWebhookHandler? Create(string eventType);
}
