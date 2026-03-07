using Streamers.Features.Categories.Features.GetCategory;

namespace Streamers.Features.IntegrationTests.Categories;

public class GetCategoryTests : BaseIntegrationTest
{
    public GetCategoryTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Get_ShouldGetCategory()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var categoryId = await CreateCategory();
        var getCommand = new GetCategory(categoryId);

        // Act
        var category = await Sender.Send(getCommand);

        // Assert
        Assert.NotNull(category);
    }
}
