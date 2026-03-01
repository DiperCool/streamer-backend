using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Vods.Dtos;
using Streamers.Features.Vods.Exceptions;

namespace Streamers.Features.Vods.Features.GetVodSettings;

public record GetVodSettings : IRequest<VodSettingsDto>;

public class GetVodSettingsHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<GetVodSettings, VodSettingsDto>
{
    public async Task<VodSettingsDto> Handle(
        GetVodSettings request,
        CancellationToken cancellationToken
    )
    {
        var settings = await streamerDbContext.VodSettings.FirstOrDefaultAsync(
            x => x.Streamer.Id == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (settings == null)
        {
            throw new VodSettingsNotFoundException();
        }
        return new VodSettingsDto { Id = settings.Id, VodEnabled = settings.VodEnabled };
    }
}
