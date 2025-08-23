using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Chats.GraphQl;

[ObjectType<ChatMessageDto>]
public static partial class ChatMessageType
{
    public static async Task<StreamerDto?> GetSenderAsync(
        [Parent(nameof(ChatMessageDto.SenderId))] ChatMessageDto chatMessage,
        IStreamersByIdDataLoader dataLoader
    )
    {
        return await dataLoader.LoadAsync(chatMessage.SenderId);
    }

    public static async Task<ChatMessageDto?> GetReplyAsync(
        [Parent(nameof(ChatMessageDto.ReplyId))] ChatMessageDto chatMessage,
        IChatMessagesByIdDataLoader dataLoader
    )
    {
        if (chatMessage.ReplyId == null)
        {
            return null;
        }
        return await dataLoader.LoadAsync(chatMessage.ReplyId.Value);
    }
}
