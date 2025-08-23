using GreenDonut;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.Features.GetPinnedMessagesByIds;

namespace Streamers.Features.Chats.GraphQl;

public static partial class PinnedMessagesDataLoader
{
    [DataLoader]
    public static async Task<IDictionary<Guid, PinnedChatMessageDto>> GetPinnedMessagesByIdAsync(
        IReadOnlyList<Guid> ids,
        IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(new GetPinnedMessagesByIds(ids), cancellationToken);
        return result;
    }
}
