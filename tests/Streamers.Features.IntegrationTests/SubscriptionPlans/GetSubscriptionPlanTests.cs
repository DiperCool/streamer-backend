using System;
using System.Threading.Tasks;
using FluentAssertions;
using Streamers.Features.SubscriptionPlans.Features.GetSubscriptionPlan;
using Streamers.Features.SubscriptionPlans.Models;

namespace Streamers.Features.IntegrationTests.SubscriptionPlans;

public class GetSubscriptionPlanTests : BaseIntegrationTest
{
    public GetSubscriptionPlanTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetSubscriptionPlan_PlanExists_ReturnsPlan()
    {
        // Arrange
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var plan = new SubscriptionPlan(streamer.Id, "prod_1", "price_1", "Test Plan", 10);
        await DbContext.SubscriptionPlans.AddAsync(plan);
        await DbContext.SaveChangesAsync();

        var query = new GetSubscriptionPlan(plan.Id);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(plan.Id);
        result.Name.Should().Be("Test Plan");
        result.Price.Should().Be(10);
        result.StreamerId.Should().Be(streamer.Id);
    }

    [Fact]
    public async Task GetSubscriptionPlan_PlanDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var nonExistentPlanId = Guid.NewGuid();
        var query = new GetSubscriptionPlan(nonExistentPlanId);

        // Act
        var act = async () => await Sender.Send(query);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
