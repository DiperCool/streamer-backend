using Shared.Abstractions.Cqrs;
using Streamers.Features.Files;
using Streamers.Features.Profiles.Models;
using Streamers.Features.Settings.Models;
using Streamers.Features.Shared.Data;
using Streamers.Features.Shared.Persistence;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Streamers.Features.CreateUser;

public record CreateUserResponse(string Id);

public record CreateUser(string Id, string Username, string Email, DateTime CreatedAt)
    : IRequest<CreateUserResponse>;

public class CreateUserHandler(StreamerDbContext context)
    : IRequestHandler<CreateUser, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(
        CreateUser request,
        CancellationToken cancellationToken
    )
    {
        Streamer streamer = new Streamer(
            request.Id,
            request.Username,
            request.Email,
            new Profile()
            {
                OfflineStreamBanner = Images.BannerObject,
                ChannelBanner = Images.ChannelObject,
            },
            new Setting(),
            request.CreatedAt,
            Images.AvatarObject
        );
        await context.Streamers.AddAsync(streamer, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return new CreateUserResponse(streamer.Id);
    }
}
