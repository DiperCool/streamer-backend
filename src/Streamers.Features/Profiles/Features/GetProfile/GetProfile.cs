using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Profiles.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Profiles.Features.GetProfile;

public record GetProfile(string StreamerId) : IRequest<ProfileDto>;

public class GetProfileHandler(StreamerDbContext context) : IRequestHandler<GetProfile, ProfileDto>
{
    public async Task<ProfileDto> Handle(GetProfile request, CancellationToken cancellationToken)
    {
        var profile = await context
            .Profiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.StreamerId == request.StreamerId, cancellationToken);

        if (profile == null)
            throw new Exception($"Profile for user {request.StreamerId} not found");

        return new ProfileDto
        {
            Bio = profile.Bio,
            ChannelBanner = profile.ChannelBanner,
            OfflineStreamBanner = profile.OfflineStreamBanner,
            Instagram = profile.Instagram,
            Youtube = profile.Youtube,
            Discord = profile.Discord,
            StreamerId = profile.StreamerId,
        };
    }
}
