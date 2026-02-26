using HotChocolate;
using HotChocolate.Types;
using Microsoft.AspNetCore.Authorization;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Features.CreateMessage;
using Streamers.Features.Chats.Features.DeleteMessage;
using Streamers.Features.Chats.Features.PinMessage;
using Streamers.Features.Chats.Features.UnpinMessage;
using Streamers.Features.Chats.Features.UpdateChatSettings;

namespace Streamers.Features.Chats.GraphQl;

[MutationType]
public static partial class ChatMutations
{
    [HotChocolate.Authorization.Authorize]
    public static async Task<PinMessageResponse> PinMessage(
        PinMessage pinMessage,
        IMediator mediator
    )
    {
        return await mediator.Send(pinMessage);
    }

    [HotChocolate.Authorization.Authorize]
    public static async Task<UnpinMessageResponse> UnpinMessage(
        UnpinMessage request,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(request);
    }

    [HotChocolate.Authorization.Authorize]
    public static async Task<UpdateChatSettingsResponse> UpdateChatSettings(
        UpdateChatSettings request,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(request);
    }

    [HotChocolate.Authorization.Authorize]
    public static async Task<CreateMessageResponse> CreateMessage(
        CreateMessage request,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(request);
    }

    [HotChocolate.Authorization.Authorize]
    public static async Task<DeleteMessageResponse> DeleteMessage(
        DeleteMessage request,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(request);
    }
}
