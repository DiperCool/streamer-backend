using System;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Streamers.Features.Chats.Models;
using Streamers.Features.Streamers.Models;
using Streamers.Features.UnitTests.Streamers.Models;
using Xunit;

namespace Streamers.Features.UnitTests.Chats.Models;

public class PinnedChatMessageTests
{
    [Fact]
    public void RaisePinnedEvent_ShouldRaiseChatMessagePinnedEvent()
    {
        // Arrange
        var streamer = Substitute.For<Streamer>();
        streamer.Id.Returns("streamer-id");
        var message = Substitute.For<ChatMessage>();
        message.Id.Returns(Guid.NewGuid());
        
        var pinnedChatMessage = new PinnedChatMessage(Guid.NewGuid(), message, streamer, DateTime.UtcNow);
        pinnedChatMessage.ClearDomainEvents();

        // Act
        pinnedChatMessage.RaisePinnedEvent("streamer-id");

        // Assert
        pinnedChatMessage.DomainEvents.Should().HaveCount(1);
        var domainEvent = pinnedChatMessage.DomainEvents.First() as ChatMessagePinnedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.StreamerId.Should().Be("streamer-id");
        domainEvent.ModeratorId.Should().Be(streamer.Id);
        domainEvent.ChatMessageId.Should().Be(message.Id);
    }

    [Fact]
    public void RaiseUnpinnedEvent_ShouldRaiseChatMessageUnpinnedEvent()
    {
        // Arrange
        var streamer = Substitute.For<Streamer>();
        var message = Substitute.For<ChatMessage>();
        message.Id.Returns(Guid.NewGuid());

        var pinnedChatMessage = new PinnedChatMessage(Guid.NewGuid(), message, streamer, DateTime.UtcNow);
        pinnedChatMessage.ClearDomainEvents();

        // Act
        pinnedChatMessage.RaiseUnpinnedEvent("streamer-id", "moderator-id");

        // Assert
        pinnedChatMessage.DomainEvents.Should().HaveCount(1);
        var domainEvent = pinnedChatMessage.DomainEvents.First() as ChatMessageUnpinnedEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.StreamerId.Should().Be("streamer-id");
        domainEvent.ModeratorId.Should().Be("moderator-id");
        domainEvent.ChatMessageId.Should().Be(message.Id);
    }
}
