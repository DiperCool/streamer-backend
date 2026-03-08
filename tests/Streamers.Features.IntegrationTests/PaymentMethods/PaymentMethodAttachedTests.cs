using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Streamers.Features.PaymentMethods.Features.AttachePaymentMethod;

namespace Streamers.Features.IntegrationTests.PaymentMethods;

public class PaymentMethodAttachedTests : BaseIntegrationTest
{
    public PaymentMethodAttachedTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task PaymentMethodAttached_ShouldCreatePaymentMethod()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        streamer.Customer.MarkAsSuccess("cus_123");

        await DbContext.SaveChangesAsync();

        var command = new PaymentMethodAttached("pm_123", "cus_123", "visa", "4242", 12, 2030);

        // Act
        await Sender.Send(command);

        // Assert
        var paymentMethod = await DbContext.PaymentMethods.FirstOrDefaultAsync(pm =>
            pm.StreamerId == streamer.Id
        );
        Assert.NotNull(paymentMethod);
        Assert.Equal(command.StripePaymentMethodId, paymentMethod.StripePaymentMethodId);
        Assert.Equal(command.CardBrand, paymentMethod.CardBrand);
        Assert.Equal(command.CardLast4, paymentMethod.CardLast4);
        Assert.Equal(command.CardExpMonth, paymentMethod.CardExpMonth);
        Assert.Equal(command.CardExpYear, paymentMethod.CardExpYear);
    }
}
