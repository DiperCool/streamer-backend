using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Banners.Features.RemoveBanner;

public record RemoveBannerResponse(Guid Id);

public record RemoveBanner(string StreamerId, Guid BannerId) : IRequest<RemoveBannerResponse>;

public class RemoveBannerHandler(
    StreamerDbContext streamerDbContext,
    IRoleService roleService,
    ICurrentUser currentUser
) : IRequestHandler<RemoveBanner, RemoveBannerResponse>
{
    public async Task<RemoveBannerResponse> Handle(
        RemoveBanner request,
        CancellationToken cancellationToken
    )
    {
        if (!await roleService.HasRole(request.StreamerId, currentUser.UserId, Permissions.Banners))
        {
            throw new UnauthorizedAccessException();
        }
        var banner = await streamerDbContext.Banners.FirstOrDefaultAsync(
            x => x.Id == request.BannerId,
            cancellationToken: cancellationToken
        );
        if (banner == null)
        {
            throw new InvalidOperationException(
                $"Could not find banner with id: {request.BannerId}"
            );
        }
        streamerDbContext.Banners.Remove(banner);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new RemoveBannerResponse(banner.Id);
    }
}
