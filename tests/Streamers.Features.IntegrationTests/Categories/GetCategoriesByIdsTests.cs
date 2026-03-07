using Streamers.Features.Categories.Features.GetCategoriesByIds;

namespace Streamers.Features.IntegrationTests.Categories;

public class GetCategoriesByIdsTests : BaseIntegrationTest
{
    public GetCategoriesByIdsTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetCategoriesByIds_ShouldReturnCategories()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var categoryId = await CreateCategory("cat1");
        var categoryId2 = await CreateCategory("cat2");
        var command = new GetCategoriesByIds([categoryId, categoryId2]);

        // Act
        var categories = await Sender.Send(command);

        // Assert
        Assert.NotNull(categories);
        Assert.Equal(2, categories.Categories.Count);
    }
}
