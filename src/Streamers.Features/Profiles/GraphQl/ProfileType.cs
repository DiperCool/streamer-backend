using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Profiles.Dtos;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;

namespace Streamers.Features.Profiles.GraphQl;

[ObjectType<ProfileDto>]
public static partial class ProfileType
{
    public static async Task<StreamerDto?> GetStreamerAsync(
        [Parent(nameof(ProfileDto.StreamerId))] ProfileDto profile,
        IStreamersByIdDataLoader dataLoader
    )
    {
        return await dataLoader.LoadAsync(profile.StreamerId);
    }
}
