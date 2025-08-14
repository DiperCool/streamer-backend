using HotChocolate;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Profiles.Dtos;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.Features.GetStreamer;

namespace Streamers.Features.Profiles.GraphQl;

[ObjectType<ProfileDto>]
public static partial class ProfileType
{
    public static async Task<StreamerDto> GetStreamerAsync(
        [Parent(nameof(ProfileDto.StreamerId))] ProfileDto profile,
        IMediator mediator
    )
    {
        return await mediator.Send(new GetStreamer(profile.StreamerId));
    }
}
