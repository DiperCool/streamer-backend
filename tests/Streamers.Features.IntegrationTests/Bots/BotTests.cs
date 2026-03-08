using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Bots.Enums;
using Streamers.Features.Bots.Features.CreateBot;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.SystemRoles.Enums;
using Xunit;

namespace Streamers.Features.IntegrationTests.Bots;

public class BotTests : BaseIntegrationTest
{
    public BotTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task CreateBot_ShouldCreateBot()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());

        var command = new CreateBot(streamer.Id, BotState.Active, "https://stream.video/url");

        // Act
        var response = await Sender.Send(command);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();

        var botInDb = await DbContext.Bots.FirstOrDefaultAsync(b => b.Id == response.Id);
        botInDb.Should().NotBeNull();
        botInDb!.StreamerId.Should().Be(streamer.Id);
        botInDb.State.Should().Be(BotState.Active);
        botInDb.StreamVideoUrl.Should().Be("https://stream.video/url");
    }

    [Fact]
    public async Task CreateBot_ShouldFailWhenUserIsNotAdmin()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(streamer.Id); // Not an admin

        var command = new CreateBot(streamer.Id, BotState.Active, "https://stream.video/url");

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
