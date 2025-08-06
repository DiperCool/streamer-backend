using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Data;
using Streamers.Features.Shared.Persistence;

namespace Streamers.Features.Profiles.Features.UpdateProfile;

public record UpdateProfileResponse(Guid Id);

public record UpdateProfile(string? Instagram, string? Youtube, string? Discord)
    : IRequest<UpdateProfileResponse>;

public class UpdateProfileHandler(StreamerDbContext context, ICurrentUser currentUser)
    : IRequestHandler<UpdateProfile, UpdateProfileResponse>
{
    public async Task<UpdateProfileResponse> Handle(
        UpdateProfile request,
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

        profile.Instagram = request.Instagram;
        profile.Youtube = request.Youtube;
        profile.Discord = request.Discord;

        await context.SaveChangesAsync(cancellationToken);

        return new UpdateProfileResponse(profile.Id);
    }
}
