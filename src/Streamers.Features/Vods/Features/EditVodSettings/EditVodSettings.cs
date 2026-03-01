using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Vods.Exceptions;

namespace Streamers.Features.Vods.Features.EditVodSettings;

public record EditVodSettingsResponse(Guid Id);

public record EditVodSettings(bool VodEnabled) : IRequest<EditVodSettingsResponse>;

public class EditVodSettingsHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<EditVodSettings, EditVodSettingsResponse>
{
    public async Task<EditVodSettingsResponse> Handle(
        EditVodSettings request,
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
        settings.VodEnabled = request.VodEnabled;
        streamerDbContext.VodSettings.Update(settings);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new EditVodSettingsResponse(settings.Id);
    }
}
