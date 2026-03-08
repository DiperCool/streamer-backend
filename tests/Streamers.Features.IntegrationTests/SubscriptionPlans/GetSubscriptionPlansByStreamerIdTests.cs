using System;
using System.Threading.Tasks;
using FluentAssertions;
using Streamers.Features.SubscriptionPlans.Features.GetSubscriptionPlansByStreamerId;
using Streamers.Features.SubscriptionPlans.Models;

namespace Streamers.Features.IntegrationTests.SubscriptionPlans;

public class GetSubscriptionPlansByStreamerIdTests : BaseIntegrationTest
{
    public GetSubscriptionPlansByStreamerIdTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetSubscriptionPlansByStreamerId_StreamerExistsWithPlans_ReturnsPlans()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var plan1 = new SubscriptionPlan(streamer.Id, "prod_1", "price_1", "Plan 1", 10);
        var plan2 = new SubscriptionPlan(streamer.Id, "prod_2", "price_2", "Plan 2", 20);

        await DbContext.SubscriptionPlans.AddRangeAsync(plan1, plan2);
        await DbContext.SaveChangesAsync();

        var query = new GetSubscriptionPlansByStreamerId(streamer.Id);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Plan 1");
        result.Should().Contain(p => p.Name == "Plan 2");
    }

    [Fact]
    public async Task GetSubscriptionPlansByStreamerId_StreamerExistsWithNoPlans_ReturnsEmpty()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var query = new GetSubscriptionPlansByStreamerId(streamer.Id);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetSubscriptionPlansByStreamerId_StreamerDoesNotExist_ReturnsEmpty()
    {
        // Arrange
        var nonExistentStreamerId = "non-existent-streamer";
        var query = new GetSubscriptionPlansByStreamerId(nonExistentStreamerId);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
