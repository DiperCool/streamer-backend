using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Services;
using Streamers.Features.Streams.Exceptions;
using Streamers.Features.Streams.Models;

namespace Streamers.Features.Streams.Features.UpdateStreamSettings;

public record UpdateStreamSettingsResponse(Guid Id);

public record UpdateStreamSettings : IRequest<UpdateStreamSettingsResponse>;

public class UpdateStreamSettingsHandler(
    StreamerDbContext dbContext,
    ICurrentUser currentUser,
    IStreamKeyGenerator keyGenerator
) : IRequestHandler<UpdateStreamSettings, UpdateStreamSettingsResponse>
{
    public async Task<UpdateStreamSettingsResponse> Handle(
        UpdateStreamSettings request,
        CancellationToken cancellationToken
    )
    {
        StreamSettings? settings = await dbContext.StreamSettings.FirstOrDefaultAsync(
            x => x.StreamerId == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (settings == null)
        {
            throw new StreamSettingsNotFoundException(currentUser.UserId);
        }
        keyGenerator.GenerateKey(settings);
        dbContext.StreamSettings.Update(settings);
        await dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
        return new UpdateStreamSettingsResponse(settings.Id);
    }
}
