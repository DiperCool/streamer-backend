using Streamers.Features.Categories.Features.CreateCategory;

namespace Streamers.Features.IntegrationTests.Categories;

public class CategoryTests : BaseIntegrationTest
{
    public CategoryTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Create_ShouldCreateCategory()
    {
        // Arrange
        await CreateAdmin();
        var command = new CreateCategory("Test", "https://test.com/image.png");

        // Act
        var categoryId = await Sender.Send(command);

        // Assert
        var category = DbContext.Categories.FirstOrDefault(p => p.Id == categoryId.Id);

        Assert.NotNull(category);
        Assert.Equal(command.Title, category.Title);
        Assert.Equal(command.Image, category.Image);
    }
}
