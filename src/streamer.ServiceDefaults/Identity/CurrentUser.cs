using Microsoft.AspNetCore.Http;

namespace streamer.ServiceDefaults.Identity;

public class CurrentUser(HttpContextAccessor contextAccessor) : ICurrentUser
{
    HttpContextAccessor _contextAccessor = contextAccessor;

    public bool IsAuthenticated { get; }
    public Guid? UserId { get; }
}
