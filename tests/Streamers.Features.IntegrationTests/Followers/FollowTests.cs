using System;
using System.Threading.Tasks;
using FluentAssertions;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Streamers.Features.Followers.Exceptions;
using Streamers.Features.Followers.Features.Follow;
using Streamers.Features.Followers.Models;
using Streamers.Features.Streamers.Exceptions;

namespace Streamers.Features.IntegrationTests.Followers;

public class FollowTests : BaseIntegrationTest
{
    public FollowTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Follow_ValidRequest_CreatesFollowerAndIncrementsFollowerCount()
    {
        // Arrange
        var followerStreamer = await CreateStreamer(Guid.NewGuid().ToString());
        var streamerToFollow = await CreateStreamer(Guid.NewGuid().ToString());

        CurrentUser.MakeAuthenticated(followerStreamer.Id);

        var command = new Follow(streamerToFollow.Id);

        // Act
        var response = await Sender.Send(command);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().NotBeEmpty();

        var follower = await DbContext.Followers.FirstAsync(f => f.Id == response.Id);
        follower.Should().NotBeNull();
        follower.FollowerStreamerId.Should().Be(followerStreamer.Id);
        follower.StreamerId.Should().Be(streamerToFollow.Id);

        var updatedStreamerToFollow = await DbContext.Streamers.FirstAsync(s =>
            s.Id == streamerToFollow.Id
        );
        await DbContext.Entry(updatedStreamerToFollow).ReloadAsync();
        updatedStreamerToFollow.Followers.Should().Be(1);

        // Verify that the background job was enqueued
        base.Factory.BackgroundJobClient.Received(1)
            .Create(Arg.Any<Job>(), Arg.Any<EnqueuedState>());
    }

    [Fact]
    public async Task Follow_AlreadyFollowing_ThrowsAlreadyFollowingException()
    {
        // Arrange
        var followerStreamer = await CreateStreamer(Guid.NewGuid().ToString());
        var streamerToFollow = await CreateStreamer(Guid.NewGuid().ToString());

        CurrentUser.MakeAuthenticated(followerStreamer.Id);

        // Create initial follow
        var initialFollow = new Follower(followerStreamer, streamerToFollow, DateTime.UtcNow);
        await DbContext.Followers.AddAsync(initialFollow);
        await DbContext.SaveChangesAsync();

        var command = new Follow(streamerToFollow.Id);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<AlreadyFollowingException>();
    }

    [Fact]
    public async Task Follow_SelfFollow_ThrowsFollowerLoopException()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(streamer.Id);

        var command = new Follow(streamer.Id);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<FollowerLoopException>();
    }

    [Fact]
    public async Task Follow_FollowerNotFound_ThrowsStreamerNotFoundException()
    {
        // Arrange
        var streamerToFollow = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated("non-existent-follower");

        var command = new Follow(streamerToFollow.Id);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should()
            .ThrowAsync<StreamerNotFoundException>()
            .WithMessage("Streamer with id 'non-existent-follower' was not found.");
    }

    [Fact]
    public async Task Follow_StreamerToFollowNotFound_ThrowsStreamerNotFoundException()
    {
        // Arrange
        var followerStreamer = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(followerStreamer.Id);

        var command = new Follow("non-existent-streamer");

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should()
            .ThrowAsync<StreamerNotFoundException>()
            .WithMessage("Streamer with id 'non-existent-streamer' was not found.");
    }
}
