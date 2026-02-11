using Streamers.Features.Chats.Models;
using Streamers.Features.Customers.Models;
using Streamers.Features.Files;
using Streamers.Features.Notifications.Models;
using Streamers.Features.Partners.Models;
using Streamers.Features.Profiles.Models;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Models;
using Streamers.Features.Settings.Models;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Models;
using Streamers.Features.StreamInfos.Models;
using Streamers.Features.Streams.Models;
using Streamers.Features.Vods.Models;

namespace Streamers.Features.Streamers.Services;

public interface IStreamerFabric
{
    public Task<Streamer> CreateStreamer(
        string id,
        string username,
        string email,
        DateTime created
    );
}

public class StreamerFabric(IStreamKeyGenerator generator, StreamerDbContext streamerDbContext)
    : IStreamerFabric
{
    public async Task<Streamer> CreateStreamer(
        string id,
        string username,
        string email,
        DateTime created
    )
    {
        var streamSettings = new StreamSettings();
        var chatSettings = new ChatSettings();
        var streamInfo = new StreamInfo("My First Stream", "English");
        Streamer streamer = new Streamer(
            id,
            username,
            email,
            new Profile()
            {
                OfflineStreamBanner = Images.BannerObject,
                ChannelBanner = Images.ChannelObject,
            },
            new Setting(),
            streamSettings,
            new Chat(chatSettings),
            created,
            Images.AvatarObject,
            chatSettings,
            streamInfo,
            new NotificationSettings() { StreamerLive = true, UserFollowed = true },
            new VodSettings() { VodEnabled = true },
            new Partner(),
            new Customer()
        );
        var role = new Role(
            streamer,
            RoleType.Broadcaster,
            streamer,
            DateTime.UtcNow,
            Permissions.All
        );

        await streamerDbContext.Streamers.AddAsync(streamer);
        await streamerDbContext.Roles.AddAsync(role);
        generator.GenerateKey(streamer.StreamSettings);

        return streamer;
    }
}
