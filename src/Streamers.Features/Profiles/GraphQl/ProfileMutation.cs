using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Profiles.Features.UpdateBio;
using Streamers.Features.Profiles.Features.UpdateChannelBanner;
using Streamers.Features.Profiles.Features.UpdateOfflineBanner;
using Streamers.Features.Profiles.Features.UpdateProfile;

namespace Streamers.Features.Profiles.GraphQl;

[MutationType]
public static partial class ProfileMutation
{
    [Authorize]
    public static async Task<UpdateProfileResponse> UpdateProfile(
        UpdateProfile input,
        IMediator mediator
    )
    {
        return await mediator.Send(input);
    }

    [Authorize]
    public static async Task<UpdateBioResponse> UpdateBio(UpdateBio input, IMediator mediator)
    {
        return await mediator.Send(input);
    }

    [Authorize]
    public static async Task<UpdateOfflineBannerResponse> UpdateOfflineBanner(
        UpdateOfflineBanner input,
        IMediator mediator
    )
    {
        return await mediator.Send(input);
    }

    [Authorize]
    public static async Task<UpdateChannelBannerResponse> UpdateChannelBanner(
        UpdateChannelBanner input,
        IMediator mediator
    )
    {
        return await mediator.Send(input);
    }
}
