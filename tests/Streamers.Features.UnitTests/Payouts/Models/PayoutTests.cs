using System;
using FluentAssertions;
using Streamers.Features.Payouts.Enums;
using Streamers.Features.Payouts.Models;
using Xunit;

namespace Streamers.Features.UnitTests.Payouts.Models;

public class PayoutTests
{
    private Payout CreatePayout()
    {
        return new Payout(
            Guid.NewGuid(),
            "streamer-id",
            "stripe-payout-id",
            100.0m,
            "usd",
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow
        );
    }

    [Fact]
    public void MakePaid_ShouldSetStatusToPaidAndNullifyFailureMessage()
    {
        // Arrange
        var payout = CreatePayout();
        payout.MakeFailed("initial failure");

        // Act
        payout.MakePaid();

        // Assert
        payout.Status.Should().Be(PayoutStatus.Paid);
        payout.FailureMessage.Should().BeNull();
    }

    [Fact]
    public void MakeFailed_ShouldSetStatusToFailedAndSetFailureMessage()
    {
        // Arrange
        var payout = CreatePayout();
        var failureMessage = "Bank account not found";

        // Act
        payout.MakeFailed(failureMessage);

        // Assert
        payout.Status.Should().Be(PayoutStatus.Failed);
        payout.FailureMessage.Should().Be(failureMessage);
    }
}
