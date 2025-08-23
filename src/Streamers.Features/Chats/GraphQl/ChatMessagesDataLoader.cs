using GreenDonut;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.Features.GetMessagesByIds;

namespace Streamers.Features.Chats.GraphQl;

public static partial class ChatMessagesDataLoader
{
    [DataLoader]
    public static async Task<IDictionary<Guid, ChatMessageDto>> GetChatMessagesByIdDataLoader(
        IReadOnlyList<Guid> ids,
        IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(new GetMessagesByIds(ids), cancellationToken);
        return result;
    }
}
