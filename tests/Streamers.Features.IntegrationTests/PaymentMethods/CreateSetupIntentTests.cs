using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Streamers.Features.PaymentMethods.Features.CreateSetupIntent;

namespace Streamers.Features.IntegrationTests.PaymentMethods;

public class CreateSetupIntentTests : BaseIntegrationTest
{
    public CreateSetupIntentTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task CreateSetupIntent_ShouldReturnClientSecret()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        streamer.Customer.MarkAsSuccess("cus_123");
        await DbContext.SaveChangesAsync();
        CurrentUser.MakeAuthenticated(streamer.Id);

        var clientSecret = "seti_123_secret_456";
        StripeService
            .CreateSetupIntentAsync("cus_123", CancellationToken.None)
            .ReturnsForAnyArgs(clientSecret);

        var command = new CreateSetupIntent();

        // Act
        var response = await Sender.Send(command);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(clientSecret, response.ClientSecret);
    }
}
