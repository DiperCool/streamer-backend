using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Payouts.Dtos;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;

namespace Streamers.Features.Payouts.Graphql;

[ObjectType<PayoutDto>]
public static partial class PayoutType
{
    public static async Task<StreamerDto?> GetStreamerAsync(
        [Parent] PayoutDto payout,
        IStreamersByIdDataLoader dataLoader
    )
    {
        return await dataLoader.LoadAsync(payout.StreamerId);
    }
}
