using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Bots.Dtos;
using Streamers.Features.Bots.Features.GetBot;
using Streamers.Features.Bots.Features.GetBots;

namespace Streamers.Features.Bots.Graphql;

[Authorize]
[QueryType]
public static partial class BotQueries
{
    [UsePaging(MaxPageSize = 15)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<BotDto>> GetBots(
        string? search,
        [Service] IMediator mediator,
        QueryContext<BotDto> rcontext,
        PagingArguments offsetPagingArguments,
        CancellationToken cancellationToken
    )
    {
        var result = await mediator.Send(
            new GetBots(search, offsetPagingArguments, rcontext),
            cancellationToken
        );
        return result.ToConnection();
    }

    public static async Task<BotDto> GetgBot(Guid id, [Service] IMediator mediator)
    {
        var response = await mediator.Send(new GetBot(id));
        return response;
    }
}
