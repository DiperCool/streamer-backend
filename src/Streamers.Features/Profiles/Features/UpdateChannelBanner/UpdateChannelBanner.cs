using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Profiles.Exceptions;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Profiles.Features.UpdateChannelBanner;

public record UpdateChannelBannerResponse(Guid Id);

public record UpdateChannelBanner(string ChannelBanner) : IRequest<UpdateChannelBannerResponse>;

public class UpdateChannelBannerHandler(StreamerDbContext context, ICurrentUser currentUser)
    : IRequestHandler<UpdateChannelBanner, UpdateChannelBannerResponse>
{
    public async Task<UpdateChannelBannerResponse> Handle(
        UpdateChannelBanner request,
        CancellationToken cancellationToken
    )
    {
        var profile = await context.Profiles.FirstOrDefaultAsync(
            p => p.StreamerId == currentUser.UserId,
            cancellationToken
        );

        if (profile == null)
        {
            throw new ProfileNotFoundException(currentUser.UserId);
        }

        profile.ChannelBanner = request.ChannelBanner;

        await context.SaveChangesAsync(cancellationToken);

        return new UpdateChannelBannerResponse(profile.Id);
    }
}
