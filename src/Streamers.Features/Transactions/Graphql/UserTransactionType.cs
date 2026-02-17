using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;
using Streamers.Features.Transactions.Dtos;

namespace Streamers.Features.Transactions.Graphql;

[ObjectType<UserTransactionDto>]
public static partial class UserTransactionType
{
    public static async Task<StreamerDto?> GetStreamerAsync(
        [Parent] UserTransactionDto transaction,
        IStreamersByIdDataLoader dataLoader
    )
    {
        return await dataLoader.LoadAsync(transaction.StreamerId);
    }
}
