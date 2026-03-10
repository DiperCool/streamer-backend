using FluentAssertions;
using Streamers.Features.Customers.Enums;
using Streamers.Features.Customers.Models;
using Xunit;

namespace Streamers.Features.UnitTests.Customers.Models;

public class CustomerTests
{
    [Fact]
    public void MarkAsSuccess_ShouldSetStripeCustomerIdAndStatus()
    {
        // Arrange
        var customer = new Customer();
        var stripeCustomerId = "cus_12345";

        // Act
        customer.MarkAsSuccess(stripeCustomerId);

        // Assert
        customer.StripeCustomerId.Should().Be(stripeCustomerId);
        customer.StripeCustomerCreationStatus.Should().Be(StripeCustomerCreationStatus.Success);
    }

    [Fact]
    public void MarkAsFailed_ShouldSetStatusToFailed()
    {
        // Arrange
        var customer = new Customer();
        customer.MarkAsSuccess("cus_12345");

        // Act
        customer.MarkAsFailed();

        // Assert
        customer.StripeCustomerCreationStatus.Should().Be(StripeCustomerCreationStatus.Failed);
    }
}
