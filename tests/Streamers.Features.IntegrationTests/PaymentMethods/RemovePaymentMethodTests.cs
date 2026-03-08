using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Streamers.Features.Customers.Models;
using Streamers.Features.PaymentMethods.Features.RemovePaymentMethod;
using Streamers.Features.PaymentMethods.Models;
using Stripe;
using PaymentMethod = Streamers.Features.PaymentMethods.Models.PaymentMethod;

namespace Streamers.Features.IntegrationTests.PaymentMethods;

public class RemovePaymentMethodTests : BaseIntegrationTest
{
    public RemovePaymentMethodTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task RemovePaymentMethod_ShouldRemovePaymentMethod()
    {
        // Arrange
        var streamer = await CreateStreamer();
        var stripeCustomerId = "cus_123";
        streamer.Customer.MarkAsSuccess(stripeCustomerId);
        await DbContext.SaveChangesAsync();
        CurrentUser.MakeAuthenticated(streamer.Id);

        var stripePaymentMethodId = "pm_123";
        var paymentMethod = new PaymentMethod(
            stripePaymentMethodId,
            streamer.Id,
            "visa",
            "4242",
            12,
            2030
        );

        await DbContext.PaymentMethods.AddAsync(paymentMethod);
        await DbContext.SaveChangesAsync();

        StripeService
            .DetachPaymentMethodAsync(
                stripeCustomerId,
                stripePaymentMethodId,
                CancellationToken.None
            )
            .Returns(true);

        var command = new RemovePaymentMethod(paymentMethod.Id);

        // Act
        var response = await Sender.Send(command);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(paymentMethod.Id, response.Id);
    }

    [Fact]
    public async Task RemovePaymentMethod_ShouldThrowStripeException_WhenStripeFails()
    {
        // Arrange
        var streamer = await CreateStreamer();
        var stripeCustomerId = "cus_123";
        streamer.Customer.MarkAsSuccess(stripeCustomerId);
        await DbContext.SaveChangesAsync();
        CurrentUser.MakeAuthenticated(streamer.Id);

        var stripePaymentMethodId = "pm_123";
        var paymentMethod = new PaymentMethod(
            stripePaymentMethodId,
            streamer.Id,
            "visa",
            "4242",
            12,
            2030
        );

        await DbContext.PaymentMethods.AddAsync(paymentMethod);
        await DbContext.SaveChangesAsync();

        StripeService
            .DetachPaymentMethodAsync(
                stripeCustomerId,
                stripePaymentMethodId,
                CancellationToken.None
            )
            .ThrowsAsync(new StripeException("Stripe error"));

        var command = new RemovePaymentMethod(paymentMethod.Id);

        // Act & Assert
        await Assert.ThrowsAsync<StripeException>(() => Sender.Send(command));
    }
}
