using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GreenDonut.Data;
using Streamers.Features.Followers.Dtos;
using Streamers.Features.Followers.Features.GetMyFollowers;
using Streamers.Features.Followers.Models;

namespace Streamers.Features.IntegrationTests.Followers;

public class GetMyFollowersTests : BaseIntegrationTest
{
    public GetMyFollowersTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetMyFollowers_UserHasFollowers_ReturnsCorrectFollowers()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var follower1 = await CreateStreamer("follower1");
        var follower2 = await CreateStreamer("follower2");

        CurrentUser.MakeAuthenticated(streamer.Id);

        // Make follower1 and follower2 follow 'streamer'
        await DbContext.Followers.AddAsync(
            new Follower(follower1, streamer, DateTime.UtcNow.AddHours(-2))
        );
        await DbContext.Followers.AddAsync(
            new Follower(follower2, streamer, DateTime.UtcNow.AddHours(-1))
        );
        await DbContext.SaveChangesAsync();

        var query = new GetMyFollowers(
            null,
            new QueryContext<FollowerDto>(),
            new PagingArguments()
        );

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.Items.Should().Contain(f => f.FollowerStreamerId == follower1.Id);
        result.Items.Should().Contain(f => f.FollowerStreamerId == follower2.Id);
    }

    [Fact]
    public async Task GetMyFollowers_UserHasNoFollowers_ReturnsEmptyPage()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(streamer.Id);

        var query = new GetMyFollowers(
            null,
            new QueryContext<FollowerDto>(),
            new PagingArguments()
        );

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMyFollowers_UserNotFound_ReturnsEmptyPage()
    {
        // Arrange
        CurrentUser.MakeAuthenticated("non-existent-user");

        var query = new GetMyFollowers(
            null,
            new QueryContext<FollowerDto>(),
            new PagingArguments()
        );

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetMyFollowers_WithSearchTerm_ReturnsFilteredFollowers()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var follower1 = await CreateStreamer("Alice");
        var follower2 = await CreateStreamer("Bob");
        var follower3 = await CreateStreamer("Charlie");

        CurrentUser.MakeAuthenticated(streamer.Id);

        // Make followers follow 'streamer'
        await DbContext.Followers.AddAsync(
            new Follower(follower1, streamer, DateTime.UtcNow.AddHours(-3))
        );
        await DbContext.Followers.AddAsync(
            new Follower(follower2, streamer, DateTime.UtcNow.AddHours(-2))
        );
        await DbContext.Followers.AddAsync(
            new Follower(follower3, streamer, DateTime.UtcNow.AddHours(-1))
        );
        await DbContext.SaveChangesAsync();

        var query = new GetMyFollowers(
            "Alice", // Search for "Alice"
            new QueryContext<FollowerDto>(),
            new PagingArguments()
        );

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(1);
        result.Items.First().FollowerStreamerId.Should().Be(follower1.Id);
    }
}
