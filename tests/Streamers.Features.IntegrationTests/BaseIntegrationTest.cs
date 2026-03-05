using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Streamers.Services;
using Streamers.Features.SystemRoles.Enums;
using Streamers.Features.SystemRoles.Models;

namespace Streamers.Features.IntegrationTests;

public abstract class BaseIntegrationTest
    : IClassFixture<StreamerWebApplicationFactory>,
        IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly IMediator Sender;
    protected readonly StreamerDbContext DbContext;

    protected BaseIntegrationTest(StreamerWebApplicationFactory factory)
    {
        _scope = factory.Services.CreateScope();

        Sender = _scope.ServiceProvider.GetRequiredService<IMediator>();
        DbContext = _scope.ServiceProvider.GetRequiredService<StreamerDbContext>();
    }

    public async Task CreateAdmin()
    {
        var streamerFabric = _scope.ServiceProvider.GetRequiredService<IStreamerFabric>();

        Streamer streamer = await streamerFabric.CreateStreamer(
            "id-admin",
            "test",
            "sdfsdf@gmail.com",
            DateTime.UtcNow
        );
        await DbContext.SystemRoles.AddAsync(
            new SystemRole(streamer, SystemRoleType.Administrator)
        );
        await DbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        DbContext?.Dispose();
    }
}
