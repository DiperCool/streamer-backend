using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.AspNetCore.Subscriptions.Protocols;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shared.Auth0.Services;
using streamer.ServiceDefaults;

namespace Streamers.Features.Shared.GraphQl;

public class AuthenticatedSocketSessionInterceptor : DefaultSocketSessionInterceptor
{
    private readonly IConfiguration _configuration;
    private readonly Auth0Options _auth0Options;
    public static readonly string PayloadAuthKey = "Authorization";

    public AuthenticatedSocketSessionInterceptor(IConfiguration configuration)
    {
        _configuration = configuration;
        _auth0Options = _configuration.BindOptions<Auth0Options>();
    }

    public override async ValueTask<ConnectionStatus> OnConnectAsync(
        ISocketSession session,
        IOperationMessagePayload connectionInitMessage,
        CancellationToken cancellationToken = default
    )
    {
        var context = session.Connection.HttpContext;
        if (context is null)
            return ConnectionStatus.Accept();

        if (!connectionInitMessage.TryGetValue(PayloadAuthKey, out object? tokenObj))
            return ConnectionStatus.Accept();

        string? token = tokenObj?.ToString();
        if (string.IsNullOrWhiteSpace(token))
            return ConnectionStatus.Accept();

        if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            token = token.Substring(7); // убираем "Bearer "

        // валидируем токен и получаем ClaimsPrincipal
        var principal = await ValidateTokenAsync(token);
        if (principal == null)
            return ConnectionStatus.Accept();

        context.User = principal;
        return ConnectionStatus.Accept();
    }

    private async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var discoveryUrl = $"{_auth0Options.AuthUrl}/.well-known/openid-configuration";

        using var http = new HttpClient();

        // Получаем OpenID Connect discovery документ
        OpenIdConnectDiscovery? discovery;
        try
        {
            var discoveryJson = await http.GetStringAsync(discoveryUrl);
            discovery = JsonSerializer.Deserialize<OpenIdConnectDiscovery>(
                discoveryJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            if (discovery == null || string.IsNullOrEmpty(discovery.JwksUri))
                return null;
        }
        catch
        {
            return null;
        }

        // Получаем JWKS
        JsonWebKeySet keys;
        try
        {
            var jwksJson = await http.GetStringAsync(discovery.JwksUri);
            keys = new JsonWebKeySet(jwksJson);
        }
        catch
        {
            return null;
        }

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _configuration["Jwt:Authority"],
            ValidateAudience = true,
            ValidAudience = _configuration["Jwt:Audience"],
            ValidateLifetime = true,
            IssuerSigningKeys = keys.Keys,
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var _);
            return principal;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return null;
        }
    }
}

// вспомогательные модели для OpenID Connect discovery
public class OpenIdConnectDiscovery
{
    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; set; } = default!;
}

public static class OperationMessagePayloadExtensions
{
    public static bool TryGetValue(
        this IOperationMessagePayload payload,
        string key,
        out object? value
    )
    {
        value = null;
        if (payload == null)
        {
            return false;
        }
        IDictionary<string, object?>? dict = payload.As<IDictionary<string, object?>>();
        if (dict != null && dict.TryGetValue(key, out value))
        {
            return true;
        }
        return false;
    }
}
