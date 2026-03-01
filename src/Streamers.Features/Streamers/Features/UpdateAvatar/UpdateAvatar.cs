using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Exceptions;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Streamers.Features.UpdateAvatar;

public record UpdateAvatarResponse(string File);

public record UpdateAvatar(string File) : IRequest<UpdateAvatarResponse>;

public class UpdateAvatarHandler(StreamerDbContext context, ICurrentUser currentUser)
    : IRequestHandler<UpdateAvatar, UpdateAvatarResponse>
{
    public async Task<UpdateAvatarResponse> Handle(
        UpdateAvatar request,
        CancellationToken cancellationToken
    )
    {
        var streamerId = currentUser.UserId;
        Streamer? streamer = await context.Streamers.FirstOrDefaultAsync(
            x => x.Id == streamerId,
            cancellationToken: cancellationToken
        );
        if (streamer == null)
        {
            throw new StreamerNotFoundException(streamerId);
        }
        streamer.Avatar = request.File;
        await context.SaveChangesAsync(cancellationToken: cancellationToken);
        return new UpdateAvatarResponse(streamer.Avatar);
    }
}
