using System.Text.Json.Serialization;
using streamer.EventBus.Events;

namespace Streamers.Api.Streamers.Events;

public record UserCreated([property: JsonPropertyName("@class")] string Class,
    [property: JsonPropertyName("time")] long Time,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("realmId")] string RealmId,
    [property: JsonPropertyName("clientId")] string ClientId,
    [property: JsonPropertyName("userId")] Guid UserId,
    [property: JsonPropertyName("ipAddress")] string IpAddress,
    [property: JsonPropertyName("details")] UserCreatedDetails Details) : IntegrationEvent;
public record UserCreatedDetails( 
    [property: JsonPropertyName("auth_method")] string AuthMethod,
    [property: JsonPropertyName("auth_type")] string AuthType,
    [property: JsonPropertyName("register_method")] string RegisterMethod,
    [property: JsonPropertyName("last_name")] string LastName,
    [property: JsonPropertyName("redirect_uri")] string RedirectUri,
    [property: JsonPropertyName("first_name")] string FirstName,
    [property: JsonPropertyName("code_id")] string CodeId,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("username")] string Username
);
