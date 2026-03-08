using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Streamers.Features.PaymentMethods.Dtos;
using Streamers.Features.PaymentMethods.Models;

namespace Streamers.Features.IntegrationTests.PaymentMethods;

public class PaymentMethodCreatedEventHandlerTests : BaseIntegrationTest
{
    public PaymentMethodCreatedEventHandlerTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task PaymentMethodCreatedHandler_ShouldSendEventToTopic()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        streamer.Customer.MarkAsSuccess("cus_123");
        await DbContext.SaveChangesAsync();
        CurrentUser.MakeAuthenticated(streamer.Id);

        var paymentMethod = new PaymentMethod("pm_123", streamer.Id, "visa", "4242", 12, 2030);

        // Act
        await DbContext.PaymentMethods.AddAsync(paymentMethod);
        await DbContext.SaveChangesAsync(); // This should trigger the domain event

        // Assert
        await base
            .Factory.MockTopicEventSender.Received(1)
            .SendAsync(
                $"{nameof(PaymentMethodCreated)}-{paymentMethod.StreamerId}",
                Arg.Is<PaymentMethodDto>(dto =>
                    dto.Id == paymentMethod.Id
                    && dto.CardBrand == paymentMethod.CardBrand
                    && dto.CardLast4 == paymentMethod.CardLast4
                ),
                CancellationToken.None
            );
    }
}
