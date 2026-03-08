using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
// Added
using Streamers.Features.Chats.Exceptions; // Added
using Streamers.Features.Chats.Features.CreateMessage;
using Streamers.Features.Chats.Features.DeleteMessage;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Roles.Models;

namespace Streamers.Features.IntegrationTests.Chats;

public class DeleteMessageTests : BaseIntegrationTest
{
    public DeleteMessageTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task DeleteMessage_ValidRequest_DeletesChatMessage()
    {
        // Arrange
        var sender = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(sender.Id);

        // Create a message as the sender
        var createMessageCommand = new CreateMessage(sender.Chat.Id, "Message to delete", null);
        var messageResponse = await Sender.Send(createMessageCommand);
        var messageId = messageResponse.MessageId;

        // Mock IRoleService to allow sender to delete their own message (Permissions.Chat is for chat owner/admin)
        // base.Factory.MockRoleService.HasRole(sender.Chat.StreamerId, sender.Id, Permissions.Chat)
        //     .Returns(true);

        var command = new DeleteMessage(messageId);

        // Act
        var response = await Sender.Send(command);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(messageId);

        var deletedMessage = await DbContext.ChatMessages.FirstAsync(m => m.Id == messageId);
        deletedMessage.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteMessage_MessageNotFound_ThrowsMessageNotFoundException()
    {
        // Arrange
        var sender = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(sender.Id);

        // base.Factory.MockRoleService.HasRole(
        //         Arg.Any<string>(),
        //         Arg.Any<string>(),
        //         Arg.Any<Permissions>()
        //     )
        //     .Returns(true);

        var nonExistentMessageId = Guid.NewGuid();
        var command = new DeleteMessage(nonExistentMessageId);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<MessageNotFoundException>();
    }

    [Fact]
    public async Task DeleteMessage_UserIsNotMessageSender_ThrowsForbiddenException()
    {
        // Arrange
        var sender = await CreateStreamer(Guid.NewGuid().ToString());
        var otherUser = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(otherUser.Id);

        // Create a message as the sender
        var createMessageCommand = new CreateMessage(sender.Chat.Id, "Message to delete", null);
        var messageResponse = await Sender.Send(createMessageCommand);
        var messageId = messageResponse.MessageId;

        // Mock IRoleService to deny otherUser from deleting the message
        // base.Factory.MockRoleService.HasRole(sender.Chat.StreamerId, otherUser.Id, Permissions.Chat)
        //     .Returns(false);

        var command = new DeleteMessage(messageId);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task DeleteMessage_AdminDeletesMessage_DeletesChatMessage()
    {
        // Arrange
        var sender = await CreateStreamer(Guid.NewGuid().ToString());
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);

        // Create a message as the sender
        var createMessageCommand = new CreateMessage(sender.Chat.Id, "Message to delete", null);
        var messageResponse = await Sender.Send(createMessageCommand);
        var messageId = messageResponse.MessageId;

        // Mock IRoleService to allow admin to delete any message
        // base.Factory.MockRoleService.HasRole(sender.Chat.StreamerId, admin.Id, Permissions.Chat)
        //     .Returns(true); // Admin has chat permission
        await DbContext.Roles.AddAsync(
            new Role(
                admin, // streamer (who has the role)
                RoleType.Administrator, // type of role
                sender, // broadcaster (for whose chat the role applies)
                DateTime.UtcNow, // createdAt
                Permissions.Chat // permissions
            )
        );
        await DbContext.SaveChangesAsync();

        var command = new DeleteMessage(messageId);

        // Act
        var response = await Sender.Send(command);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(messageId);

        var deletedMessage = await DbContext.ChatMessages.FirstAsync(m => m.Id == messageId);
        deletedMessage.IsDeleted.Should().BeTrue();
    }
}
