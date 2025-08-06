using Microsoft.AspNetCore.Http;

namespace streamer.ServiceDefaults.Identity;

public class CurrentUser(IHttpContextAccessor contextAccessor) : ICurrentUser
{
    public string UserId =>
        contextAccessor.HttpContext?.User.GetUserId()
        ?? throw new InvalidOperationException("User is not authenticated");

    public bool IsAuthenticated =>
        contextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
