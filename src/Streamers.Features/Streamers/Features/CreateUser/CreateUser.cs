using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Models;
using Streamers.Features.Files;
using Streamers.Features.Profiles.Models;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Models;
using Streamers.Features.Settings.Models;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Streamers.Services;
using Streamers.Features.Streams.Models;

namespace Streamers.Features.Streamers.Features.CreateUser;

public record CreateUserResponse(string Id);

public record CreateUser(string Id, string Username, string Email, DateTime CreatedAt)
    : IRequest<CreateUserResponse>;

public class CreateUserHandler(StreamerDbContext context, IStreamKeyGenerator streamKeyGenerator)
    : IRequestHandler<CreateUser, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(
        CreateUser request,
        CancellationToken cancellationToken
    )
    {
        var streamSettings = new StreamSettings();
        var chatSettings = new ChatSettings();
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
            streamSettings,
            new Chat(chatSettings),
            request.CreatedAt,
            Images.AvatarObject,
            chatSettings
        );
        var role = new Role(
            streamer,
            RoleType.Broadcaster,
            streamer,
            DateTime.UtcNow,
            Permissions.All
        );
        streamKeyGenerator.GenerateKey(streamSettings);

        await context.Streamers.AddAsync(streamer, cancellationToken);
        await context.Roles.AddAsync(role, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return new CreateUserResponse(streamer.Id);
    }
}
