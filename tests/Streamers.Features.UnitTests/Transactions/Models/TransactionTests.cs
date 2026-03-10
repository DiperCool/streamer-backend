using System;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Transactions.Models;
using Xunit;

namespace Streamers.Features.UnitTests.Transactions.Models;

public class TransactionTests
{
    private Transaction CreateTransaction()
    {
        var user = Substitute.For<Streamer>();
        var streamer = Substitute.For<Streamer>();

        return new Transaction(
            Guid.NewGuid(),
            "user-id",
            user,
            "streamer-id",
            streamer,
            TransactionType.Subscription,
            10.0m,
            1.0m,
            9.0m,
            TransactionStatus.Succeeded,
            DateTime.UtcNow,
            null
        );
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateStatusAndRaiseEvent()
    {
        // Arrange
        var transaction = CreateTransaction();
        var newStatus = TransactionStatus.Refunded;
        transaction.ClearDomainEvents();

        // Act
        transaction.UpdateStatus(newStatus);

        // Assert
        transaction.Status.Should().Be(newStatus);
        transaction.DomainEvents.Should().HaveCount(1);
        var domainEvent = transaction.DomainEvents.First() as TransactionUpdated;
        domainEvent.Should().NotBeNull();
        domainEvent!.Transaction.Should().Be(transaction);
    }

    [Fact]
    public void SetStripeInvoiceUrl_ShouldUpdateUrl()
    {
        // Arrange
        var transaction = CreateTransaction();
        var url = "https://invoice.stripe.com/i/123";

        // Act
        transaction.SetStripeInvoiceUrl(url);

        // Assert
        transaction.StripeInvoiceUrl.Should().Be(url);
    }
}
