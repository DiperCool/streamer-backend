using System;
using System.Threading.Tasks;
using Streamers.Features.Categories.Features.GetCategoriesWatchersByIds;

namespace Streamers.Features.IntegrationTests.Categories;

public class GetCategoriesWatchersByIdsTests : BaseIntegrationTest
{
    public GetCategoriesWatchersByIdsTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetCategoriesWatchersByIds_ShouldReturnWatchers()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var streamer = await CreateStreamer(Guid.NewGuid().ToString());
        var categoryId = await CreateCategory("cat1");
        var categoryId2 = await CreateCategory("cat2");
        await CreateStream(streamer, categoryId, 100);
        await CreateStream(streamer, categoryId2, 200);

        var command = new GetCategoriesWatchersByIds([categoryId, categoryId2]);

        // Act
        var watchers = await Sender.Send(command);

        // Assert
        Assert.NotNull(watchers);
        Assert.Equal(2, watchers.Count);
        Assert.Equal(100, watchers[categoryId]);
        Assert.Equal(200, watchers[categoryId2]);
    }
}
