using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.Exceptions;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Streamers.Features.GetStreamer;

public record GetStreamer() : IRequest<StreamerMeDto>;

public class GetStreamerHandler(StreamerDbContext context, ICurrentUser currentUser)
    : IRequestHandler<GetStreamer, StreamerMeDto>
{
    public async Task<StreamerMeDto> Handle(
        GetStreamer request,
        CancellationToken cancellationToken
    )
    {
        Streamer? streamer = await context
            .Streamers.IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.Id == currentUser.UserId,
                cancellationToken: cancellationToken
            );
        if (streamer == null)
        {
            throw new StreamerNotFoundException(currentUser.UserId);
        }

        return new StreamerMeDto
        {
            Id = streamer.Id,
            UserName = streamer.UserName,
            Avatar = streamer.Avatar,
            Followers = streamer.Followers,
            IsLive = streamer.IsLive,
            FinishedAuth = streamer.FinishedAuth,
            HasUnreadNotifications = streamer.HasUnreadNotifications,
        };
    }
}
