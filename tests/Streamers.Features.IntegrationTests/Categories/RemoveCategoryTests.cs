using Streamers.Features.Categories.Features.RemoveCategory;

namespace Streamers.Features.IntegrationTests.Categories;

public class RemoveCategoryTests : BaseIntegrationTest
{
    public RemoveCategoryTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Remove_ShouldRemoveCategory()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var categoryId = await CreateCategory();
        var removeCommand = new RemoveCategory(categoryId);

        // Act
        await Sender.Send(removeCommand);

        // Assert
        var category = DbContext.Categories.FirstOrDefault(p => p.Id == categoryId);

        Assert.Null(category);
    }
}
