using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Streamers.Features.Notifications.Dtos;
using Streamers.Features.Notifications.Models;

namespace Streamers.Features.IntegrationTests.Notifications;

public class NotificationCreatedEventHandlerTests : BaseIntegrationTest
{
    public NotificationCreatedEventHandlerTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task NotificationCreatedHandler_ShouldUpdateStreamerAndSendEventToTopic()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        streamer.HasUnreadNotifications = false; // Ensure it's false initially
        await DbContext.SaveChangesAsync();

        CurrentUser.MakeAuthenticated(streamer.Id);

        var notification = new LiveStartedNotification(streamer.Id, DateTime.UtcNow, streamer);

        // Act
        await DbContext.Notifications.AddAsync(notification);
        await DbContext.SaveChangesAsync(); // This should trigger the domain event

        // Assert that Streamer.HasUnreadNotifications is updated
        var updatedStreamer = await DbContext.Streamers.FirstAsync(s => s.Id == streamer.Id);
        await DbContext.Entry(updatedStreamer).ReloadAsync(); // Reload to get updated properties
        updatedStreamer.HasUnreadNotifications.Should().BeTrue();

        // Assert that ITopicEventSender.SendAsync was called
        await base
            .Factory.MockTopicEventSender.Received(1)
            .SendAsync(
                $"{nameof(NotificationCreated)}-{notification.UserId}",
                Arg.Is<NotificationDto>(dto =>
                    dto.Id == notification.Id && dto.StreamerId == streamer.Id
                ),
                CancellationToken.None
            );
    }
}
