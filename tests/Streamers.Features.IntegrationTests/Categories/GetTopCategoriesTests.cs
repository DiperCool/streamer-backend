using System;
using System.Threading.Tasks;
using Streamers.Features.Categories.Features.GetTopCategories;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.IntegrationTests.Categories;

public class GetTopCategoriesTests : BaseIntegrationTest
{
    public GetTopCategoriesTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetTopCategories_ShouldReturnTopCategories()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var streamer = await CreateStreamer();
        var categoryId = await CreateCategory("cat1");
        var categoryId2 = await CreateCategory("cat2");
        await CreateStream(streamer, categoryId, 100);
        await CreateStream(streamer, categoryId2, 200);

        var command = new GetTopCategories();

        // Act
        var categories = await Sender.Send(command);

        // Assert
        Assert.NotNull(categories);
        Assert.Equal(2, categories.Count);
        Assert.Equal("cat2", categories[0].Title);
        Assert.Equal("cat1", categories[1].Title);
    }
}
