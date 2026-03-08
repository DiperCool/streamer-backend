using System;
using System.Threading.Tasks;
using FluentAssertions;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Subscriptions.Features.HandleSubscriptionInvoicePaid;
using Streamers.Features.Subscriptions.Models;

namespace Streamers.Features.IntegrationTests.Subscriptions;

public class HandleSubscriptionInvoicePaidTests : BaseIntegrationTest
{
    public HandleSubscriptionInvoicePaidTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    private async Task<Subscription> CreateSubscription(
        SubscriptionStatus status,
        Streamer user,
        Streamer streamer
    )
    {
        var subscription = new Subscription(
            Guid.NewGuid(),
            user.Id,
            streamer.Id,
            Guid.NewGuid().ToString(),
            status,
            DateTime.UtcNow,
            "Test Subscription"
        );
        await DbContext.Subscriptions.AddAsync(subscription);
        await DbContext.SaveChangesAsync();
        return subscription;
    }

    [Fact]
    public async Task HandleSubscriptionInvoicePaid_SubscriptionExistsAndIsIncomplete_UpdatesSubscriptionToActive()
    {
        // Arrange
        var user = await CreateStreamer(Guid.NewGuid().ToString());
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var subscription = await CreateSubscription(SubscriptionStatus.Incomplete, user, streamer);

        var newPeriodEnd = DateTime.UtcNow.AddMonths(1);
        var command = new HandleSubscriptionInvoicePaid(
            subscription.StripeSubscriptionId,
            newPeriodEnd
        );

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.Should().BeTrue();
        await DbContext.Entry(subscription).ReloadAsync();
        subscription.Status.Should().Be(SubscriptionStatus.Active);
        subscription.IsCurrent.Should().BeTrue();
        subscription
            .CurrentPeriodEnd.Should()
            .BeCloseTo(newPeriodEnd, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task HandleSubscriptionInvoicePaid_SubscriptionExistsAndIsNotIncomplete_UpdatesSubscription()
    {
        // Arrange
        var user = await CreateStreamer(Guid.NewGuid().ToString());
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var subscription = await CreateSubscription(SubscriptionStatus.Active, user, streamer);
        subscription.SucceedPayment(); // Increment streak
        await DbContext.SaveChangesAsync();

        var newPeriodEnd = DateTime.UtcNow.AddMonths(1);
        var command = new HandleSubscriptionInvoicePaid(
            subscription.StripeSubscriptionId,
            newPeriodEnd
        );
        var initialStreak = subscription.CurrentStreak;

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.Should().BeTrue();
        await DbContext.Entry(subscription).ReloadAsync();
        subscription.Status.Should().Be(SubscriptionStatus.Active);
        subscription.IsCurrent.Should().BeFalse(); // SucceedPayment doesn't set IsCurrent
        subscription.CurrentStreak.Should().Be(initialStreak + 1); // SucceedPayment is called again
        subscription
            .CurrentPeriodEnd.Should()
            .BeCloseTo(newPeriodEnd, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task HandleSubscriptionInvoicePaid_SubscriptionDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var nonExistentStripeSubscriptionId = "non-existent-stripe-sub";
        var newPeriodEnd = DateTime.UtcNow.AddMonths(1);
        var command = new HandleSubscriptionInvoicePaid(
            nonExistentStripeSubscriptionId,
            newPeriodEnd
        );

        // Act
        var result = await Sender.Send(command);

        // Assert
        result.Should().BeFalse();
    }
}
