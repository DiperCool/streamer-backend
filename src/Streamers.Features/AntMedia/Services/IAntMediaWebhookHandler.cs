namespace Streamers.Features.AntMedia.Services;

public class AntMediaWebhookPayload
{
    public string Id { get; set; }
    public string Action { get; set; }
    public string? StreamName { get; set; }
    public DateTime Timestamp { get; set; }
    public string? VodId { get; set; }
}

public interface IAntMediaWebhookHandler
{
    Task HandleAsync(AntMediaWebhookPayload payload);
}
