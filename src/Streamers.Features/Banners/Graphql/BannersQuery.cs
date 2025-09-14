using HotChocolate;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Banners.Dtos;
using Streamers.Features.Banners.Features.GetBanners;

namespace Streamers.Features.Banners.Graphql;

[QueryType]
public static partial class BannersQuery
{
    public static async Task<List<BannerDto>> GetBanners(
        string streamerId,
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(new GetBanners(streamerId));
    }
}
