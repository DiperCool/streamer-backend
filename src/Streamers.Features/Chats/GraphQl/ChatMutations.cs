using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Features.CreateMessage;
using Streamers.Features.Chats.Features.DeleteMessage;
using Streamers.Features.Chats.Features.PinMessage;
using Streamers.Features.Chats.Features.UnpinMessage;
using Streamers.Features.Chats.Features.UpdateChatSettings;

namespace Streamers.Features.Chats.GraphQl;

[MutationType]
[Authorize]
public static partial class ChatMutations
{
    public static async Task<PinMessageResponse> PinMessage(
        PinMessage pinMessage,
        IMediator mediator
    )
    {
        return await mediator.Send(pinMessage);
    }

    public static async Task<UnpinMessageResponse> UnpinMessage(
        UnpinMessage request,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(request);
    }

    public static async Task<UpdateChatSettingsResponse> UpdateChatSettings(
        UpdateChatSettings request,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(request);
    }

    public static async Task<CreateMessageResponse> CreateMessage(
        CreateMessage request,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(request);
    }

    public static async Task<DeleteMessageResponse> DeleteMessage(
        DeleteMessage request,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(request);
    }
}
