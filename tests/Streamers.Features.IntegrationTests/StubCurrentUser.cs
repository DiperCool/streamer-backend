using streamer.ServiceDefaults.Identity;

namespace Streamers.Features.IntegrationTests;

public class StubCurrentUser : ICurrentUser
{
    public bool IsAuthenticated { get; private set; }
    public string UserId { get; private set; } = string.Empty;

    public void MakeAnonymous()
    {
        IsAuthenticated = false;
        UserId = string.Empty;
    }

    public void MakeAuthenticated(string userId)
    {
        IsAuthenticated = true;
        UserId = userId;
    }
}
