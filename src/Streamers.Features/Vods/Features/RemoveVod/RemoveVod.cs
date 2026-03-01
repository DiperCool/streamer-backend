using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Vods.Enums;
using Streamers.Features.Vods.Exceptions;

namespace Streamers.Features.Vods.Features.RemoveVod;

public record RemoveVodResponse(Guid Id);

public record RemoveVod(Guid Id) : IRequest<RemoveVodResponse>;

public class RemoveVodHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<RemoveVod, RemoveVodResponse>
{
    public async Task<RemoveVodResponse> Handle(
        RemoveVod request,
        CancellationToken cancellationToken
    )
    {
        var role = await streamerDbContext.Roles.FirstOrDefaultAsync(
            x => x.StreamerId == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        var query = streamerDbContext.Vods.AsNoTracking();
        if (role == null || !role.Permissions.HasPermission(Permissions.Roles))
        {
            query = query.Where(x => x.Type == VodType.Public);
        }
        var vod = await query.FirstOrDefaultAsync(
            x => x.Id == request.Id,
            cancellationToken: cancellationToken
        );
        if (vod == null)
        {
            throw new VodNotFoundException(request.Id);
        }
        streamerDbContext.Vods.Remove(vod);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new RemoveVodResponse(vod.Id);
    }
}
