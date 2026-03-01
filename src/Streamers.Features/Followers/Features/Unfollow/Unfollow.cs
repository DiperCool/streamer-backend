using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Followers.Exceptions;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Followers.Features.Unfollow;

public record UnfollowResponse(Guid Id);

public record Unfollow(string StreamerId) : IRequest<UnfollowResponse>;

public class UnfollowHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<Unfollow, UnfollowResponse>
{
    public async Task<UnfollowResponse> Handle(
        Unfollow request,
        CancellationToken cancellationToken
    )
    {
        var following = await streamerDbContext.Followers.FirstOrDefaultAsync(
            x => x.StreamerId == request.StreamerId && x.FollowerStreamerId == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (following == null)
        {
            throw new AlreadyUnfollowingException();
        }
        await streamerDbContext
            .Streamers.Where(s => s.Id == request.StreamerId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(s => s.Followers, s => s.Followers - 1),
                cancellationToken
            );
        streamerDbContext.Followers.Remove(following);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new UnfollowResponse(following.Id);
    }
}
