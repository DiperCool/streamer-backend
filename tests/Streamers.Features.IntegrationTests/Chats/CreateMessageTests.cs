using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Streamers.Features.Chats.Exceptions;
using Streamers.Features.Chats.Features.CreateMessage;
using Streamers.Features.Chats.Models;
using Streamers.Features.Chats.Services;
using Streamers.Features.Shared.Cqrs.Behaviours;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Streamers.Exceptions;
using MyValidationException = Streamers.Features.Shared.Cqrs.Behaviours.ValidationException;

namespace Streamers.Features.IntegrationTests.Chats;

public class CreateMessageTests : BaseIntegrationTest
{
    public CreateMessageTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task CreateMessage_ValidRequest_CreatesChatMessage()
    {
        // Arrange
        var sender = await CreateStreamer(Guid.NewGuid().ToString());
        // chatOwner is not needed here since Chat is created with sender

        CurrentUser.MakeAuthenticated(sender.Id);

        var command = new CreateMessage(sender.Chat.Id, "Hello chat!", null);

        // Act
        var response = await Sender.Send(command);

        // Assert
        response.Should().NotBeNull();
        response.MessageId.Should().NotBeEmpty();

        var chatMessage = await DbContext.ChatMessages.FirstAsync(m => m.Id == response.MessageId);
        chatMessage.Should().NotBeNull();
        chatMessage.Sender.Id.Should().Be(sender.Id);
        chatMessage.Message.Should().Be("Hello chat!");
        chatMessage.Chat.Id.Should().Be(sender.Chat.Id);
    }

    [Fact]
    public async Task CreateMessage_SenderNotFound_ThrowsStreamerNotFoundException()
    {
        // Arrange
        // chatOwner and chat are not needed here

        CurrentUser.MakeAuthenticated("non-existent-sender");
        // Mock the permission service to avoid issues if a chat is found


        var command = new CreateMessage(Guid.NewGuid(), "Hello chat!", null); // Use a random chat ID

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should()
            .ThrowAsync<StreamerNotFoundException>()
            .WithMessage("Streamer with id 'non-existent-sender' was not found.");
    }

    [Fact]
    public async Task CreateMessage_ChatNotFound_ThrowsChatNotFoundException()
    {
        // Arrange
        var sender = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(sender.Id);

        var command = new CreateMessage(Guid.NewGuid(), "Hello chat!", null);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ChatNotFoundException>();
    }

    [Fact]
    public async Task CreateMessage_ReplyMessageNotFound_ThrowsMessageNotFoundException()
    {
        // Arrange
        var sender = await CreateStreamer(Guid.NewGuid().ToString());
        // chatOwner is not needed here

        CurrentUser.MakeAuthenticated(sender.Id);

        var command = new CreateMessage(sender.Chat.Id, "Hello chat!", Guid.NewGuid());

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<MessageNotFoundException>();
    }

    [Fact]
    public async Task CreateMessage_MessageTooLong_ThrowsValidationException()
    {
        // Arrange
        var sender = await CreateStreamer(Guid.NewGuid().ToString());

        CurrentUser.MakeAuthenticated(sender.Id);

        var longMessage = new string('a', 251); // Message > 250 chars
        var command = new CreateMessage(sender.Chat.Id, longMessage, null);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<MyValidationException>();
    }
}
