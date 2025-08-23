using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;

namespace Streamers.Features.Chats.GraphQl;

[ObjectType<PinnedChatMessageDto>]
public static partial class PinnedChatMessageType
{
    public static async Task<StreamerDto?> GetPinnedBy(
        [Parent(nameof(PinnedChatMessageDto.PinnedById))] PinnedChatMessageDto message,
        IStreamersByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        return await dataLoader.LoadAsync(message.PinnedById, cancellationToken);
    }

    public static async Task<ChatMessageDto?> GetMessage(
        [Parent(nameof(PinnedChatMessageDto.MessageId))] PinnedChatMessageDto message,
        IChatMessagesByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        return await dataLoader.LoadAsync(message.MessageId, cancellationToken);
    }
}
