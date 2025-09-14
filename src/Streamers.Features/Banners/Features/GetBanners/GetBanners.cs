using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Banners.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Banners.Features.GetBanners;

public record GetBanners(string StreamerId) : IRequest<List<BannerDto>>;

public class GetBannersGetBannersHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetBanners, List<BannerDto>>
{
    public async Task<List<BannerDto>> Handle(
        GetBanners request,
        CancellationToken cancellationToken
    )
    {
        var result = await streamerDbContext
            .Banners.AsNoTracking()
            .Where(x => x.StreamerId == request.StreamerId)
            .Select(x => new BannerDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Image = x.Image,
                Url = x.Url,
            })
            .ToListAsync(cancellationToken: cancellationToken);
        return result;
    }
}
