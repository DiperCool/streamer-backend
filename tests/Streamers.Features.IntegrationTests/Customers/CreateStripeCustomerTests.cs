using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Streamers.Features.IntegrationTests.Customers;

public class CreateStripeCustomerTests : BaseIntegrationTest
{
    public CreateStripeCustomerTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task CreateStreamer_ShouldTriggerStripeCustomerCreation()
    {
        // Arrange
        var stripeCustomerId = "cus_12345";
        
        // Act
        var streamer = await CreateStreamer();
        await Task.Delay(1000);

        // Assert
        var updatedStreamer = await DbContext
            .Streamers.Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == streamer.Id);

        // Poll until the customer is updated or timeout
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        while (
            stopwatch.Elapsed < TimeSpan.FromSeconds(10)
            && updatedStreamer?.Customer?.StripeCustomerId == null
        )
        {
            await Task.Delay(100);
            DbContext.ChangeTracker.Clear();
            updatedStreamer = await DbContext
                .Streamers.Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.Id == streamer.Id);
        }

        Assert.NotNull(updatedStreamer);
        Assert.NotNull(updatedStreamer.Customer);
        Assert.Equal(stripeCustomerId, updatedStreamer.Customer.StripeCustomerId);
    }
}
