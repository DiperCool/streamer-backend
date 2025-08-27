using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Streamers.Features.FinishAuth;

public record FinishAuthResponse(string Id);

public record FinishAuth(string UserName) : IRequest<FinishAuthResponse>;

public class FinishAuthHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<FinishAuth, FinishAuthResponse>
{
    public async Task<FinishAuthResponse> Handle(
        FinishAuth request,
        CancellationToken cancellationToken
    )
    {
        Streamer? streamer = await streamerDbContext
            .Streamers.IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.Id == currentUser.UserId,
                cancellationToken: cancellationToken
            );
        if (streamer == null)
        {
            throw new NullReferenceException(
                $"Streamer with id {currentUser.UserId} does not exist"
            );
        }
        var exist = await streamerDbContext.Streamers.AnyAsync(
            x => x.UserName == request.UserName,
            cancellationToken: cancellationToken
        );
        if (exist)
        {
            throw new InvalidOperationException("Streamer already exists");
        }
        streamer.FinishAuth(request.UserName);
        streamerDbContext.Streamers.Update(streamer);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new FinishAuthResponse(streamer.Id);
    }
}
