using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Chats.Dtos;

namespace Streamers.Features.Chats.GraphQl;

[ObjectType<ChatDto>]
public static partial class ChatType
{
    public static async Task<ChatSettingsDto?> GetSettings(
        [Parent(nameof(ChatDto.SettingsId))] ChatDto chat,
        IChatSettingsByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        var settings = await dataLoader.LoadAsync(chat.SettingsId, cancellationToken);
        return settings;
    }

    public static async Task<Dtos.PinnedChatMessageDto?> GetPinnedMessage(
        [Parent(nameof(ChatDto.PinnedMessageId))] ChatDto chat,
        IPinnedMessagesByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        if (chat.PinnedMessageId == null)
        {
            return null;
        }
        var settings = await dataLoader.LoadAsync(chat.PinnedMessageId.Value, cancellationToken);
        return settings;
    }
}
