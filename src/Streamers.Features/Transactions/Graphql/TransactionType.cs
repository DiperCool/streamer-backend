using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;
using Streamers.Features.Transactions.Dtos;

namespace Streamers.Features.Transactions.Graphql;

[ObjectType<TransactionDto>]
public static partial class TransactionType
{
    public static async Task<StreamerDto?> GetStreamerAsync(
        [Parent] TransactionDto transaction,
        IStreamersByIdDataLoader dataLoader
    )
    {
        return await dataLoader.LoadAsync(transaction.StreamerId);
    }

    public static async Task<StreamerDto?> GetUserAsync(
        [Parent] TransactionDto transaction,
        IStreamersByIdDataLoader dataLoader
    )
    {
        return await dataLoader.LoadAsync(transaction.UserId);
    }
}
