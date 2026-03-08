using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Streamers.Features.Profiles.Features.GetProfile;
using Streamers.Features.Profiles.Models;
using Xunit;

namespace Streamers.Features.IntegrationTests.Profiles;

public class ProfileTests : BaseIntegrationTest
{
    public ProfileTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetProfile_ShouldReturnProfile()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(streamer.Id);

        var profile = new Profile
        {
            StreamerId = streamer.Id,
            Bio = "Test Bio",
            ChannelBanner = "https://example.com/channel.png",
            OfflineStreamBanner = "https://example.com/offline.png",
            Instagram = "test_insta",
            Youtube = "test_youtube",
            Discord = "test_discord",
        };
        await DbContext.Profiles.AddAsync(profile);
        await DbContext.SaveChangesAsync();

        var query = new GetProfile(streamer.Id);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.StreamerId.Should().Be(streamer.Id);
        result.Bio.Should().Be("Test Bio");
        result.ChannelBanner.Should().Be("https://example.com/channel.png");
        result.OfflineStreamBanner.Should().Be("https://example.com/offline.png");
        result.Instagram.Should().Be("test_insta");
        result.Youtube.Should().Be("test_youtube");
        result.Discord.Should().Be("test_discord");
    }
}