using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Streamers.Features.Followers.Exceptions;
using Streamers.Features.Followers.Features.Unfollow;
using Streamers.Features.Followers.Models;

namespace Streamers.Features.IntegrationTests.Followers;

public class UnfollowTests : BaseIntegrationTest
{
    public UnfollowTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Unfollow_ValidRequest_RemovesFollowerAndDecrementsFollowerCount()
    {
        // Arrange
        var followerStreamer = await CreateStreamer(Guid.NewGuid().ToString());
        var streamerToUnfollow = await CreateStreamer(Guid.NewGuid().ToString());
        streamerToUnfollow.Followers = 1; // Manually set the follower count for testing

        CurrentUser.MakeAuthenticated(followerStreamer.Id);

        // Create initial follow
        var initialFollow = new Follower(followerStreamer, streamerToUnfollow, DateTime.UtcNow);
        await DbContext.Followers.AddAsync(initialFollow);

        await DbContext.SaveChangesAsync();

        var command = new Unfollow(streamerToUnfollow.Id);

        // Act
        var response = await Sender.Send(command);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(initialFollow.Id);

        var follower = await DbContext.Followers.FirstOrDefaultAsync(f => f.Id == initialFollow.Id);
        follower.Should().BeNull(); // Follower should be removed

        var updatedStreamerToUnfollow = await DbContext.Streamers.FirstAsync(s =>
            s.Id == streamerToUnfollow.Id
        );
        await DbContext.Entry(updatedStreamerToUnfollow).ReloadAsync();
        updatedStreamerToUnfollow.Followers.Should().Be(0); // Follower count should be decremented
    }

    [Fact]
    public async Task Unfollow_NotFollowing_ThrowsAlreadyUnfollowingException()
    {
        // Arrange
        var followerStreamer = await CreateStreamer(Guid.NewGuid().ToString());
        var streamerToUnfollow = await CreateStreamer(Guid.NewGuid().ToString());

        CurrentUser.MakeAuthenticated(followerStreamer.Id);

        var command = new Unfollow(streamerToUnfollow.Id);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<AlreadyUnfollowingException>();
    }
}
