using System;
using System.Threading.Tasks;
using Streamers.Features.PaymentMethods.Features.MakePaymentMethodDefault;
using Streamers.Features.PaymentMethods.Models;

namespace Streamers.Features.IntegrationTests.PaymentMethods;

public class MakePaymentMethodDefaultTests : BaseIntegrationTest
{
    public MakePaymentMethodDefaultTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task MakePaymentMethodDefault_ShouldSetPaymentMethodAsDefault()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(streamer.Id);

        var paymentMethod1 = new PaymentMethod("pm_123", streamer.Id, "visa", "4242", 12, 2030);
        paymentMethod1.MakeDefault();

        var paymentMethod2 = new PaymentMethod(
            "pm_456",
            streamer.Id,
            "mastercard",
            "5555",
            12,
            2032
        );

        await DbContext.PaymentMethods.AddRangeAsync(paymentMethod1, paymentMethod2);
        await DbContext.SaveChangesAsync();

        var command = new MakePaymentMethodDefault(paymentMethod2.Id);

        // Act
        await Sender.Send(command);

        DbContext.ChangeTracker.Clear();

        // Assert
        var updatedPaymentMethod1 = await DbContext.PaymentMethods.FindAsync(paymentMethod1.Id);
        var updatedPaymentMethod2 = await DbContext.PaymentMethods.FindAsync(paymentMethod2.Id);

        Assert.NotNull(updatedPaymentMethod1);
        Assert.NotNull(updatedPaymentMethod2);

        Assert.False(updatedPaymentMethod1.IsDefault);
        Assert.True(updatedPaymentMethod2.IsDefault);
    }
}
