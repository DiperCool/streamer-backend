using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.Features.GetChat;
using Streamers.Features.Chats.Features.GetChatSettings;
using Streamers.Features.Chats.Features.GetMessageHistory;
using Streamers.Features.Chats.Features.GetMessages;

namespace Streamers.Features.Chats.GraphQl;

[QueryType]
public static partial class ChatQuery
{
    public static async Task<ChatDto> GetChat(string streamerId, [Service] IMediator mediator)
    {
        return await mediator.Send(new GetChat(streamerId));
    }

    [Authorize]
    public static async Task<ChatSettingsDto> GetChatSettings([Service] IMediator mediator)
    {
        return await mediator.Send(new GetChatSettings());
    }

    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<ChatMessageDto>> GetChatMessages(
        Guid chatId,
        [Service] IMediator mediator,
        QueryContext<ChatMessageDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetMessages(chatId, offsetPagingArguments, rcontext),
            cancellationToken
        );
        return result.ToConnection();
    }

    [UseFiltering]
    [UseSorting]
    public static async Task<List<ChatMessageDto>> GetChatMessagesHistory(
        Guid chatId,
        DateTime startFrom,
        [Service] IMediator mediator,
        QueryContext<ChatMessageDto> rcontext,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetMessageHistory(chatId, startFrom, rcontext),
            cancellationToken
        );
        return result;
    }
}
