using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shared.Stripe;
using Streamers.Features.Partners.Features.BecomePartner;
using Streamers.Features.Partners.Models;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Streamers.Enums;
using Streamers.Features.SystemRoles.Enums;
using Xunit;

namespace Streamers.Features.IntegrationTests.Partners;

public class PartnerTests : BaseIntegrationTest
{
    public PartnerTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task BecomePartner_ShouldStartOnboardingProcessForExistingPartner()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(streamer.Id);

        // Ensure StripeService.CreateExpressAccountAsync returns a valid account ID
        Factory.StripeService.CreateExpressAccountAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>()
        ).Returns(new CreateStripeAccountResult("acct_test"));

        var command = new BecomePartner(streamer.Id);

        // Act
        var response = await Sender.Send(command);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(streamer.Id);

        var partnerInDb = await DbContext.Partners
            .FirstOrDefaultAsync(p => p.StreamerId == streamer.Id);
        partnerInDb.Should().NotBeNull();
        partnerInDb!.StripeAccountId.Should().Be("acct_test");
        partnerInDb.StripeOnboardingStatus.Should().Be(StripeOnboardingStatus.InProgress);

        // Verify StripeService was called
        await Factory.StripeService.Received(1).CreateExpressAccountAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task BecomePartner_ShouldFailWhenUserIsNotStreamerOrAdmin()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var nonAdminUser = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(nonAdminUser.Id); // Authenticate as different user

        var command = new BecomePartner(streamer.Id);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("You are not authorized to perform this action.");
    }

    [Fact]
    public async Task BecomePartner_ShouldThrowIfStripeAccountCreationFails()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(streamer.Id);

        // Ensure StripeService.CreateExpressAccountAsync throws an exception
        Factory.StripeService.CreateExpressAccountAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>()
        ).Returns(Task.FromException<CreateStripeAccountResult>(new Exception("Stripe error")));

        var command = new BecomePartner(streamer.Id);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Stripe error");
    }
}
