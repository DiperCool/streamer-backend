namespace Shared.Auth0.Services;

public class Auth0Options
{
    public string ApiUrl { get; set; } = string.Empty;
    public string AuthUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string GrantType { get; set; } = string.Empty;
}
