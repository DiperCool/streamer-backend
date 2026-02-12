using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNetCore.CAP;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Streamers.Features.CreateUser;

namespace Streamers.Features.Streamers.EventHandlers;

public class NewUserEvent
{
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
}

public interface IUserEventHandler
{
    Task HandleNewUserAsync(NewUserEvent evt);
}

public class UserEventHandler(IMediator mediator) : IUserEventHandler, ICapSubscribe
{
    [CapSubscribe("new_users")]
    public async Task HandleNewUserAsync(NewUserEvent evt)
    {
        await mediator.Send(new CreateUser(evt.UserId, evt.Username, evt.Email, evt.CreatedAt));
    }
}
