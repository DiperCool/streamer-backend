using System;
using FluentAssertions;
using Streamers.Features.Subscriptions.Models;
using Xunit;

namespace Streamers.Features.UnitTests.Subscriptions.Models;

public class SubscriptionTests
{
    private Subscription CreateSubscription()
    {
        return new Subscription(
            Guid.NewGuid(),
            "user-id",
            "streamer-id",
            "stripe-sub-id",
            SubscriptionStatus.Incomplete,
            DateTime.UtcNow,
            "Test Subscription"
        );
    }

    [Fact]
    public void SucceedPayment_ShouldSetActiveAndIncrementStreak()
    {
        // Arrange
        var subscription = CreateSubscription();
        var initialStreak = subscription.CurrentStreak;

        // Act
        subscription.SucceedPayment();

        // Assert
        subscription.Status.Should().Be(SubscriptionStatus.Active);
        subscription.CurrentStreak.Should().Be(initialStreak + 1);
    }

    [Fact]
    public void MakeCurrentAndSucceedPayment_ShouldSetIsCurrentAndActive()
    {
        // Arrange
        var subscription = CreateSubscription();

        // Act
        subscription.MakeCurrentAndSucceedPayment();

        // Assert
        subscription.IsCurrent.Should().BeTrue();
        subscription.Status.Should().Be(SubscriptionStatus.Active);
    }

    [Fact]
    public void MakeObsolete_ShouldSetIsCurrentToFalse()
    {
        // Arrange
        var subscription = CreateSubscription();
        subscription.MakeCurrentAndSucceedPayment();

        // Act
        subscription.MakeObsolete();

        // Assert
        subscription.IsCurrent.Should().BeFalse();
    }

    [Fact]
    public void Cancel_ShouldSetStatusToCanceledAndIsCurrentToFalse()
    {
        // Arrange
        var subscription = CreateSubscription();
        subscription.MakeCurrentAndSucceedPayment();

        // Act
        subscription.Cancel();

        // Assert
        subscription.Status.Should().Be(SubscriptionStatus.Canceled);
        subscription.IsCurrent.Should().BeFalse();
    }

    [Fact]
    public void MarkAsPastDue_ShouldSetStatusToPastDue()
    {
        // Arrange
        var subscription = CreateSubscription();

        // Act
        subscription.MarkAsPastDue();

        // Assert
        subscription.Status.Should().Be(SubscriptionStatus.PastDue);
    }

    [Fact]
    public void SetCurrentPeriodEnd_ShouldUpdateCurrentPeriodEnd()
    {
        // Arrange
        var subscription = CreateSubscription();
        var newPeriodEnd = DateTime.UtcNow.AddMonths(1);

        // Act
        subscription.SetCurrentPeriodEnd(newPeriodEnd);

        // Assert
        subscription.CurrentPeriodEnd.Should().Be(newPeriodEnd);
    }
}
