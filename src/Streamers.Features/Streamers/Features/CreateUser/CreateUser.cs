using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Services;

namespace Streamers.Features.Streamers.Features.CreateUser;

public record CreateUserResponse(string Id);

[Transactional]
public record CreateUser(string Id, string Username, string Email, DateTime CreatedAt)
    : IRequest<CreateUserResponse>;

public class CreateUserHandler(StreamerDbContext context, IStreamerFabric streamerFabric)
    : IRequestHandler<CreateUser, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(
        CreateUser request,
        CancellationToken cancellationToken
    )
    {
        if (
            await context.Streamers.AnyAsync(
                x => x.Id == request.Id,
                cancellationToken: cancellationToken
            )
        )
        {
            return new CreateUserResponse(request.Id);
        }
        var streamer = await streamerFabric.CreateStreamer(
            request.Id,
            request.Username,
            request.Email,
            request.CreatedAt
        );

        await context.SaveChangesAsync(cancellationToken);
        return new CreateUserResponse(streamer.Id);
    }
}
