using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Followers.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Followers.Features.Follow;

public record FollowResponse(Guid Id);

public record Follow(string StreamerId) : IRequest<FollowResponse>;

public class FollowHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<Follow, FollowResponse>
{
    public async Task<FollowResponse> Handle(Follow request, CancellationToken cancellationToken)
    {
        var whoFollows = await streamerDbContext.Streamers.FirstOrDefaultAsync(
            x => x.Id == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (whoFollows == null)
        {
            throw new InvalidOperationException(
                $"Could not find streamer with ID {currentUser.UserId}"
            );
        }
        var streamer = await streamerDbContext.Streamers.FirstOrDefaultAsync(
            x => x.Id == request.StreamerId,
            cancellationToken: cancellationToken
        );
        if (streamer == null)
        {
            throw new InvalidOperationException(
                $"Could not find streamer with ID {request.StreamerId}"
            );
        }

        var alreadyFollows = await streamerDbContext.Followers.AnyAsync(
            x => x.FollowerStreamerId == currentUser.UserId && x.StreamerId == request.StreamerId,
            cancellationToken: cancellationToken
        );
        if (alreadyFollows)
        {
            throw new InvalidOperationException(
                $"Already following streamer with ID {request.StreamerId}"
            );
        }
        Follower follower = new Follower(whoFollows, streamer);
        await streamerDbContext
            .Streamers.Where(s => s.Id == request.StreamerId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(s => s.Followers, s => s.Followers + 1),
                cancellationToken
            );
        await streamerDbContext.Followers.AddAsync(follower, cancellationToken);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new FollowResponse(follower.Id);
    }
}
