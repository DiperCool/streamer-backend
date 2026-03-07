using GreenDonut.Data;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Categories.Features.CreateCategory;
using Streamers.Features.Categories.Features.EditCategory;
using Streamers.Features.Categories.Features.GetCategories;
using Streamers.Features.Categories.Features.GetCategory;
using Streamers.Features.Categories.Features.RemoveCategory;

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
    
    [Fact]
    public async Task Edit_ShouldEditCategory()
    {
        // Arrange
        await CreateAdmin();
        var createCommand = new CreateCategory("Test", "https://test.com/image.png");
        var categoryId = await Sender.Send(createCommand);
        var editCommand = new EditCategory(categoryId.Id, "Test 2", "https://test.com/image2.png");

        // Act
        await Sender.Send(editCommand);

        // Assert
        var category = DbContext.Categories.FirstOrDefault(p => p.Id == categoryId.Id);

        Assert.NotNull(category);
        Assert.Equal(editCommand.Title, category.Title);
        Assert.Equal(editCommand.Image, category.Image);
    }
    
    [Fact]
    public async Task Remove_ShouldRemoveCategory()
    {
        // Arrange
        await CreateAdmin();
        var createCommand = new CreateCategory("Test", "https://test.com/image.png");
        var categoryId = await Sender.Send(createCommand);
        var removeCommand = new RemoveCategory(categoryId.Id);

        // Act
        await Sender.Send(removeCommand);

        // Assert
        var category = DbContext.Categories.FirstOrDefault(p => p.Id == categoryId.Id);

        Assert.Null(category);
    }
    
    [Fact]
    public async Task Get_ShouldGetCategory()
    {
        // Arrange
        await CreateAdmin();
        var createCommand = new CreateCategory("Test", "https://test.com/image.png");
        var categoryId = await Sender.Send(createCommand);
        var getCommand = new GetCategory(categoryId.Id);

        // Act
        var category = await Sender.Send(getCommand);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(createCommand.Title, category.Title);
        Assert.Equal(createCommand.Image, category.Image);
    }
    
    [Fact]
    public async Task GetList_ShouldGetCategories()
    {
        // Arrange
        await CreateAdmin();
        var createCommand = new CreateCategory("Test", "https://test.com/image.png");
        await Sender.Send(createCommand);
        var getCommand = new GetCategories(null, null, new PagingArguments(), new QueryContext<CategoryDto>());

        // Act
        var categories = await Sender.Send(getCommand);

        // Assert
        Assert.NotNull(categories);
        Assert.Single(categories.Items);
    }
}
