using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistence;

namespace Streamers.Features.Profiles.Features.UpdateOfflineBanner;

public record UpdateOfflineBannerResponse(Guid Id);

public record UpdateOfflineBanner(string OfflineBanner) : IRequest<UpdateOfflineBannerResponse>;

public class UpdateOfflineBannerHandler(StreamerDbContext context, ICurrentUser currentUser)
    : IRequestHandler<UpdateOfflineBanner, UpdateOfflineBannerResponse>
{
    public async Task<UpdateOfflineBannerResponse> Handle(
        UpdateOfflineBanner request,
        CancellationToken cancellationToken
    )
    {
        var profile = await context.Profiles.FirstOrDefaultAsync(
            p => p.StreamerId == currentUser.UserId,
            cancellationToken
        );

        if (profile == null)
        {
            throw new Exception($"Profile for user {currentUser.UserId} not found");
        }

        profile.OfflineStreamBanner = request.OfflineBanner;

        await context.SaveChangesAsync(cancellationToken);

        return new UpdateOfflineBannerResponse(profile.Id);
    }
}
