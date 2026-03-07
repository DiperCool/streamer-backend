using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Streamers.Services;
using Streamers.Features.SystemRoles.Enums;
using Streamers.Features.SystemRoles.Models;

namespace Streamers.Features.IntegrationTests;

[Collection(nameof(FixtureCollection))]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly IServiceScope _scope;
    protected readonly IMediator Sender;
    protected readonly StreamerDbContext DbContext;
    private readonly StreamerWebApplicationFactory _factory;

    protected BaseIntegrationTest(StreamerWebApplicationFactory factory)
    {
        _factory = factory;
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
        ClearChangeTracker();
    }

    public async Task InitializeAsync()
    {
        await _factory.Respawn();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();

        _scope.Dispose();
    }

    protected void ClearChangeTracker()
    {
        DbContext.ChangeTracker.Clear();
    }
}
