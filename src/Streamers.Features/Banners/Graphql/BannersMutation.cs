using HotChocolate;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Banners.Features.CreateBanner;
using Streamers.Features.Banners.Features.RemoveBanner;
using Streamers.Features.Banners.Features.UpdateBanner;

namespace Streamers.Features.Banners.Graphql;

[MutationType]
public static partial class BannersMutation
{
    public static async Task<CreateBannerResponse> CreateBanner(
        CreateBanner banner,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(banner);
    }

    public static async Task<UpdateBannerResponse> UpdateBanner(
        UpdateBanner banner,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(banner);
    }

    public static async Task<RemoveBannerResponse> RemoveBanner(
        RemoveBanner banner,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(banner);
    }
}
