using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;

namespace Streamers.Features.Chats.GraphQl;

[ObjectType<BannedUserDto>]
public static partial class BannedUserType
{
    public static async Task<StreamerDto?> GetUser(
        [Parent(nameof(BannedUserDto.UserId))] BannedUserDto bannedUser,
        IStreamersByIdDataLoader dataLoader
    )
    {
        return await dataLoader.LoadAsync(bannedUser.UserId);
    }

    public static async Task<StreamerDto?> GetBannedBy(
        [Parent(nameof(BannedUserDto.BannedById))] BannedUserDto bannedUser,
        IStreamersByIdDataLoader dataLoader
    )
    {
        return await dataLoader.LoadAsync(bannedUser.BannedById);
    }
}
