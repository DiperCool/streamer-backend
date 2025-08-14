using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streams.Dtos;
using Streamers.Features.Streams.Models;

namespace Streamers.Features.Streams.Features.GetStreamSettings;

public record GetStreamSettings() : IRequest<StreamSettingsDto>;

public class GetStreamSettingsHandler(StreamerDbContext context, ICurrentUser currentUser)
    : IRequestHandler<GetStreamSettings, StreamSettingsDto>
{
    public async Task<StreamSettingsDto> Handle(
        GetStreamSettings request,
        CancellationToken cancellationToken
    )
    {
        StreamSettings? settings = await context.StreamSettings.FirstOrDefaultAsync(x =>
            x.StreamerId == currentUser.UserId
        );
        if (settings == null)
        {
            throw new InvalidOperationException(
                $"Could not find stream settings for user {currentUser.UserId}"
            );
        }

        return new StreamSettingsDto
        {
            Id = settings.Id,
            StreamKey = settings.StreamKey,
            StreamUrl = settings.StreamUrl,
        };
    }
}
