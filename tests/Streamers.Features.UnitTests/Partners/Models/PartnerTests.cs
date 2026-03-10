using FluentAssertions;
using Streamers.Features.Partners.Models;
using Streamers.Features.Streamers.Enums;
using Xunit;

namespace Streamers.Features.UnitTests.Partners.Models;

public class PartnerTests
{
    [Fact]
    public void StartOnboarding_ShouldSetStripeAccountIdAndStatus()
    {
        // Arrange
        var partner = new Partner();
        var stripeAccountId = "acct_12345";

        // Act
        partner.StartOnboarding(stripeAccountId);

        // Assert
        partner.StripeAccountId.Should().Be(stripeAccountId);
        partner.StripeOnboardingStatus.Should().Be(StripeOnboardingStatus.InProgress);
    }

    [Fact]
    public void CompleteOnboarding_ShouldSetStatusToCompleted()
    {
        // Arrange
        var partner = new Partner();
        partner.StartOnboarding("acct_12345");

        // Act
        partner.CompleteOnboarding();

        // Assert
        partner.StripeOnboardingStatus.Should().Be(StripeOnboardingStatus.Completed);
    }
}
