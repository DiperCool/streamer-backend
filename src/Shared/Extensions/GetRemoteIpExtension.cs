using Microsoft.AspNetCore.Http;

namespace Shared.Extensions;

public static class GetRemoteIpExtension
{
    public static string? GetClientIpAddress(this HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
        if (string.IsNullOrEmpty(forwardedFor))
        {
            return context.Connection.RemoteIpAddress?.ToString();
        }

        var ipAddresses = forwardedFor.Split(',').Select(ip => ip.Trim()).ToList();
        if (ipAddresses.Any())
        {
            return ipAddresses.First();
        }

        return context.Connection.RemoteIpAddress?.ToString();
    }
}
