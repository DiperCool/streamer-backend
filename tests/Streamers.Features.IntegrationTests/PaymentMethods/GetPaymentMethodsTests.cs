using System;
using System.Threading.Tasks;
using Streamers.Features.PaymentMethods.Features.GetPaymentMethods;
using Streamers.Features.PaymentMethods.Models;

namespace Streamers.Features.IntegrationTests.PaymentMethods;

public class GetPaymentMethodsTests : BaseIntegrationTest
{
    public GetPaymentMethodsTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetPaymentMethods_ShouldReturnPaymentMethodsForStreamer()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        CurrentUser.MakeAuthenticated(streamer.Id);

        var paymentMethod = new PaymentMethod(
            "pm_123",
            streamer.Id,
            "visa",
            "4242",
            12,
            2030
        );

        await DbContext.PaymentMethods.AddAsync(paymentMethod);
        await DbContext.SaveChangesAsync();

        var command = new GetPaymentMethods();

        // Act
        var response = await Sender.Send(command);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response.PaymentMethods);
        var dto = response.PaymentMethods[0];
        Assert.Equal(paymentMethod.Id, dto.Id);
        Assert.Equal(paymentMethod.CardBrand, dto.CardBrand);
        Assert.Equal(paymentMethod.CardLast4, dto.CardLast4);
        Assert.Equal(paymentMethod.CardExpMonth, dto.CardExpMonth);
        Assert.Equal(paymentMethod.CardExpYear, dto.CardExpYear);
        Assert.Equal(paymentMethod.IsDefault, dto.IsDefault);
    }
}
