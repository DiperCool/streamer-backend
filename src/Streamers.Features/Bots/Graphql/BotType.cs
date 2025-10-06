using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Bots.Dtos;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;

namespace Streamers.Features.Bots.Graphql;

[ObjectType<BotDto>]
public static partial class BotType
{
    public static async Task<StreamerDto?> GetStreamerAsync(
        [Parent(nameof(BotDto.StreamerId))] BotDto bot,
        [Service] IStreamersByIdDataLoader dataLoader,
        CancellationToken cancellationToken
    )
    {
        return await dataLoader.LoadAsync(bot.StreamerId, cancellationToken);
    }
}
