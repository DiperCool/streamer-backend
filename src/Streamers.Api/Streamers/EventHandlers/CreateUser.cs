using MassTransit;
using Streamers.Api.Profiles.Models;
using Streamers.Api.Settings.Models;
using Streamers.Api.Shared.Data;
using Streamers.Api.Streamers.Events;
using Streamers.Api.Streamers.Models;

namespace Streamers.Api.Streamers.EventHandlers;

public class CreateUser(StreamerDbContext streamerDbContext) : IConsumer<UserCreated>
{
    private StreamerDbContext _streamerDbContext = streamerDbContext;

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var msg = context.Message;
        Profile profile = new Profile();
        Setting setting = new Setting();
        Streamer streamer = new Streamer(msg.UserId, msg.Details.Username, msg.Details.Email, msg.Details.FirstName, msg.Details.LastName, profile, setting);
        await _streamerDbContext.Streamers.AddAsync(streamer);
        await _streamerDbContext.SaveChangesAsync();
    }
}
