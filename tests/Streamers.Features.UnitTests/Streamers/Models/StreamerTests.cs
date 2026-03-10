using System;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Models;
using Streamers.Features.Customers.Models;
using Streamers.Features.Notifications.Models;
using Streamers.Features.Partners.Models;
using Streamers.Features.Profiles.Models;
using Streamers.Features.Settings.Models;
using Streamers.Features.Streamers.Models;
using Streamers.Features.StreamInfos.Models;
using Streamers.Features.Streams.Models;
using Streamers.Features.Vods.Models;
using Xunit;
using Stream = Streamers.Features.Streams.Models.Stream;

namespace Streamers.Features.UnitTests.Streamers.Models;

public class StreamerTests
{
    private readonly Profile _profile = Substitute.For<Profile>();
    private readonly Setting _setting = Substitute.For<Setting>();
    private readonly StreamSettings _streamSettings = Substitute.For<StreamSettings>();
    private readonly Chat _chat = Substitute.For<Chat>();
    private readonly ChatSettings _chatSettings = Substitute.For<ChatSettings>();
    private readonly StreamInfo _streamInfo = Substitute.For<StreamInfo>();
    private readonly NotificationSettings _notificationSettings = Substitute.For<NotificationSettings>();
    private readonly VodSettings _vodSettings = Substitute.For<VodSettings>();
    private readonly Partner _partner = Substitute.For<Partner>();
    private readonly Customer _customer = Substitute.For<Customer>();

    private Streamer CreateStreamer(string id = "test-streamer")
    {
        return new Streamer(
            id,
            "test-user",
            "test@test.com",
            _profile,
            _setting,
            _streamSettings,
            _chat,
            DateTime.UtcNow,
            "avatar.jpg",
            _chatSettings,
            _streamInfo,
            _notificationSettings,
            _vodSettings,
            _partner,
            _customer
        );
    }

    [Fact]
    public void SetLive_WhenCalledWithTrueAndStream_ShouldSetIsLiveToTrueAndSetCurrentStream()
    {
        // Arrange
        var streamer = CreateStreamer();
        var stream = Substitute.For<Stream>();
        var streamId = Guid.NewGuid();
        stream.Id.Returns(streamId);
        streamer.ClearDomainEvents();

        // Act
        streamer.SetLive(true, stream);

        // Assert
        streamer.IsLive.Should().BeTrue();
        streamer.CurrentStreamId.Should().Be(streamId);
        streamer.CurrentStream.Should().Be(stream);
        streamer.DomainEvents.Should().HaveCount(1);
        var domainEvent = streamer.DomainEvents.First() as StreamerUpdated;
        domainEvent.Should().NotBeNull();
        domainEvent!.Streamer.Should().Be(streamer);
    }

    [Fact]
    public void SetLive_WhenCalledWithFalseAndNullStream_ShouldSetIsLiveToFalseAndNullifyCurrentStream()
    {
        // Arrange
        var streamer = CreateStreamer();
        streamer.SetLive(true, Substitute.For<Stream>()); // Initial state
        streamer.ClearDomainEvents();

        // Act
        streamer.SetLive(false, null);

        // Assert
        streamer.IsLive.Should().BeFalse();
        streamer.CurrentStreamId.Should().BeNull();
        streamer.CurrentStream.Should().BeNull();
        streamer.DomainEvents.Should().HaveCount(1);
        var domainEvent = streamer.DomainEvents.First() as StreamerUpdated;
        domainEvent.Should().NotBeNull();
        domainEvent!.Streamer.Should().Be(streamer);
    }
}
