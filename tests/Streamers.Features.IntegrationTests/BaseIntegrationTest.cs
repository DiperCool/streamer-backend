using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Cqrs;
using Shared.Abstractions.Domain;
using Shared.Stripe;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Categories.Features.CreateCategory;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Streamers.Services;
using Streamers.Features.Streams.Enums;
using Streamers.Features.Streams.Models;
using Streamers.Features.SystemRoles.Enums;
using Streamers.Features.SystemRoles.Models;
using Streamers.Features.Tags.Models;
using Stream = Streamers.Features.Streams.Models.Stream;

namespace Streamers.Features.IntegrationTests;

[Collection(nameof(FixtureCollection))]
public abstract class BaseIntegrationTest : IAsyncLifetime
{
    private readonly IServiceScope _scope;
    protected readonly IMediator Sender;
    protected readonly StreamerDbContext DbContext;
    private readonly StreamerWebApplicationFactory _factory;
    protected readonly StubCurrentUser CurrentUser;
    protected readonly IStripeService StripeService;

    protected BaseIntegrationTest(StreamerWebApplicationFactory factory)
    {
        _factory = factory;
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<IMediator>();
        DbContext = _scope.ServiceProvider.GetRequiredService<StreamerDbContext>();
        CurrentUser = (StubCurrentUser)_scope.ServiceProvider.GetRequiredService<ICurrentUser>();
        StripeService = factory.StripeService;
    }

    public async Task<Streamer> CreateAdmin()
    {
        var streamerFabric = _scope.ServiceProvider.GetRequiredService<IStreamerFabric>();

        Streamer streamer = await streamerFabric.CreateStreamer(
            "id-admin",
            "test-admin",
            "admin@email.com",
            DateTime.UtcNow,
            true
        );

        await DbContext.SystemRoles.AddAsync(
            new SystemRole(streamer, SystemRoleType.Administrator)
        );

        await DbContext.SaveChangesAsync();

        return streamer;
    }

    public async Task<Streamer> CreateStreamer()
    {
        var streamerFabric = _scope.ServiceProvider.GetRequiredService<IStreamerFabric>();
        var userName = Guid.NewGuid().ToString();
        var streamerId = Guid.NewGuid().ToString();
        Streamer streamer = await streamerFabric.CreateStreamer(
            streamerId,
            userName,
            $"{userName}@email.com",
            DateTime.UtcNow,
            true
        );

        await DbContext.SaveChangesAsync();

        return streamer;
    }

    public async Task<Guid> CreateCategory(string? title = null)
    {
        var categoryTitle = title ?? Guid.NewGuid().ToString();
        var command = new CreateCategory(categoryTitle, "https://test.com/image.png")
        {
            Title = categoryTitle,
            Image = "https://test.com/image.png",
        };
        var categoryId = await Sender.Send(command);
        return categoryId.Id;
    }

    public async Task<Tag> CreateTag(string? title = null)
    {
        var tag = new Tag(Guid.NewGuid(), title ?? Guid.NewGuid().ToString());
        await DbContext.Tags.AddAsync(tag);
        await DbContext.SaveChangesAsync();
        return tag;
    }

    public async Task<Stream> CreateStream(Streamer streamer, Guid categoryId, int viewers)
    {
        var category = await DbContext.Categories.FindAsync(categoryId);
        var tag = await CreateTag();

        var stream = new Stream(
            streamer,
            Guid.NewGuid().ToString(),
            "Test Stream",
            DateTime.UtcNow,
            [new StreamSource { SourceType = StreamSourceType.Hls, Url = "test" }],
            "en",
            [tag],
            null,
            category
        );
        stream.SetCurrentViewers(viewers);

        await DbContext.Streams.AddAsync(stream);
        await DbContext.SaveChangesAsync();
        return stream;
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
}
