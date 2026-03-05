using streamer.ServiceDefaults.Identity;

namespace Streamers.Features.IntegrationTests;

public class StubCurrentUser : ICurrentUser
{
    public bool IsAuthenticated { get; } = true;
    public string UserId { get; } = "id-admin";
}
