using System;
using System.Linq;
using FluentAssertions;
using Streamers.Features.PaymentMethods.Models;
using Xunit;

namespace Streamers.Features.UnitTests.PaymentMethods.Models;

public class PaymentMethodTests
{
    [Fact]
    public void Constructor_ShouldRaisePaymentMethodCreatedEvent()
    {
        // Arrange & Act
        var paymentMethod = new PaymentMethod(
            "pm_123",
            "streamer-id",
            "Visa",
            "4242",
            12,
            2030
        );

        // Assert
        paymentMethod.DomainEvents.Should().HaveCount(1);
        var domainEvent = paymentMethod.DomainEvents.First() as PaymentMethodCreated;
        domainEvent.Should().NotBeNull();
        domainEvent!.PaymentMethod.Should().Be(paymentMethod);
    }

    [Fact]
    public void Delete_ShouldRaisePaymentMethodDeletedEvent()
    {
        // Arrange
        var paymentMethod = new PaymentMethod(
            "pm_123",
            "streamer-id",
            "Visa",
            "4242",
            12,
            2030
        );
        paymentMethod.ClearDomainEvents();

        // Act
        paymentMethod.Delete();

        // Assert
        paymentMethod.DomainEvents.Should().HaveCount(1);
        var domainEvent = paymentMethod.DomainEvents.First() as PaymentMethodDeleted;
        domainEvent.Should().NotBeNull();
        domainEvent!.PaymentMethodId.Should().Be(paymentMethod.Id);
        domainEvent!.StreamerId.Should().Be("streamer-id");
    }

    [Fact]
    public void MakeDefault_ShouldSetIsDefaultToTrue()
    {
        // Arrange
        var paymentMethod = new PaymentMethod(
            "pm_123",
            "streamer-id",
            "Visa",
            "4242",
            12,
            2030
        );

        // Act
        paymentMethod.MakeDefault();

        // Assert
        paymentMethod.IsDefault.Should().BeTrue();
    }
}
