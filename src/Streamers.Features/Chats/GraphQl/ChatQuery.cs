using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.Features.GetChat;
using Streamers.Features.Chats.Features.GetChatSettings;
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

    [UsePaging(MaxPageSize = 50)]
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
}
