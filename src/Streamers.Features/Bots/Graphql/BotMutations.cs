using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Bots.Features.CreateBot;
using Streamers.Features.Bots.Features.EditBot;
using Streamers.Features.Bots.Features.RemoveBot;

namespace Streamers.Features.Bots.Graphql;

[Authorize]
[MutationType]
public static partial class BotMutations
{
    [Authorize]
    public static async Task<CreateBotResponse> CreateBot(CreateBot input, IMediator mediator)
    {
        return await mediator.Send(input);
    }

    [Authorize]
    public static async Task<EditBotResponse> EditBot(EditBot input, IMediator mediator)
    {
        return await mediator.Send(input);
    }

    [Authorize]
    public static async Task<RemoveBotResponse> RemoveBot(RemoveBot input, IMediator mediator)
    {
        return await mediator.Send(input);
    }
}

