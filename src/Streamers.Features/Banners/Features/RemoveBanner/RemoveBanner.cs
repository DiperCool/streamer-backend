using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Banners.Features.RemoveBanner;

public record RemoveBannerResponse(Guid Id);

public record RemoveBanner(string StreamerId, Guid BannerId) : IRequest<RemoveBannerResponse>;

public class RemoveBannerHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<RemoveBanner, RemoveBannerResponse>
{
    public async Task<RemoveBannerResponse> Handle(
        RemoveBanner request,
        CancellationToken cancellationToken
    )
    {
        var role = await streamerDbContext
            .Roles.Include(x => x.Broadcaster)
            .FirstOrDefaultAsync(
                x => x.StreamerId == currentUser.UserId,
                cancellationToken: cancellationToken
            );
        if (role == null)
        {
            throw new InvalidOperationException(
                $"Could not find streamer with id: {request.StreamerId}"
            );
        }

        if (!role.Permissions.HasPermission(Permissions.Banners))
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
