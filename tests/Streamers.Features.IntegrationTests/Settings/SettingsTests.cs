using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Streamers.Features.Settings.Models;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.IntegrationTests.Settings;

public class SettingsTests : BaseIntegrationTest
{
    public SettingsTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Get_ShouldReturnDefaultSettingsForNewStreamer()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());

        // Act
        var loadedStreamer = await DbContext.Streamers
            .Include(s => s.Setting)
            .FirstAsync(s => s.Id == streamer.Id);

        // Assert
        loadedStreamer.Should().NotBeNull();
        loadedStreamer.Setting.Should().NotBeNull();
        loadedStreamer.Setting.EmailNotificationsEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task Update_ShouldUpdateEmailNotificationsEnabled()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());

        // Act
        var loadedStreamer = await DbContext.Streamers
            .Include(s => s.Setting)
            .FirstAsync(s => s.Id == streamer.Id);

        loadedStreamer.Setting.EmailNotificationsEnabled = true;
        await DbContext.SaveChangesAsync();

        // Assert
        var updatedStreamer = await DbContext.Streamers
            .Include(s => s.Setting)
            .FirstAsync(s => s.Id == streamer.Id);

        updatedStreamer.Setting.Should().NotBeNull();
        updatedStreamer.Setting.EmailNotificationsEnabled.Should().BeTrue();
    }
}
