using System.Globalization;
using System.Text.Json;
using DotNetCore.CAP;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Streamers.Features.CreateUser;

namespace Streamers.Features.Streamers.EventHandlers;

public interface IUserEventHandler
{
    Task HandleNewUserAsync(JsonElement evt);
}

public class UserEventHandler(IMediator mediator) : IUserEventHandler, ICapSubscribe
{
    [CapSubscribe("new_users")]
    public async Task HandleNewUserAsync(JsonElement evt)
    {
        string email = evt.GetProperty("email").GetString() ?? "";

        string username = evt.GetProperty("username").GetString() ?? email;
        string userId = evt.GetProperty("user_id").GetString() ?? "";

        DateTime createdAt = DateTime.MinValue;
        if (
            evt.TryGetProperty("created_at", out JsonElement createdAtElement)
            && createdAtElement.ValueKind == JsonValueKind.String
            && DateTime.TryParse(
                createdAtElement.GetString(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out DateTime parsedDate
            )
        )
        {
            createdAt = parsedDate;
        }

        await mediator.Send(new CreateUser(userId, username, email, createdAt));
    }
}
