using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Auth0.Services;
using Shared.Seeds;
using streamer.ServiceDefaults;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Services;
using Streamers.Features.SystemRoles.Enums;
using Streamers.Features.SystemRoles.Models;

namespace Streamers.Features.Streamers.Seed;

public class AdminSeed(
    StreamerDbContext streamerDbContext,
    IConfiguration conifugaration,
    IAuth0Service auth0Service,
    IStreamerFabric fabric
) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        if (await streamerDbContext.Streamers.IgnoreQueryFilters().AnyAsync())
        {
            return;
        }

        var options = conifugaration.BindOptions<DefaultAdminOptions>();
        var streamer = await fabric.CreateStreamer(
            "auth0|" + options.Id,
            options.UserName,
            options.Email,
            DateTime.UtcNow
        );
        if (!await auth0Service.UserExists(streamer.Id))
        {
            await auth0Service.CreateUser(
                options.Id,
                options.Email,
                options.UserName,
                options.Password
            );
        }

        var systemRole = new SystemRole(streamer, SystemRoleType.Administrator);
        await streamerDbContext.SystemRoles.AddAsync(systemRole);
        await streamerDbContext.SaveChangesAsync();
    }

    public int Order => 0;
}
