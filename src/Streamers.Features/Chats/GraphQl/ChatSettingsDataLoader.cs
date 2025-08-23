using GreenDonut;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.Features.GetChatSettingsByIds;

namespace Streamers.Features.Chats.GraphQl;

public static partial class ChatSettingsDataLoader
{
    [DataLoader]
    public static async Task<IDictionary<Guid, ChatSettingsDto>> GetChatSettingsByIdAsync(
        IReadOnlyList<Guid> ids,
        IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(new GetChatSettingsByIds(ids), cancellationToken);
        return result;
    }
}
