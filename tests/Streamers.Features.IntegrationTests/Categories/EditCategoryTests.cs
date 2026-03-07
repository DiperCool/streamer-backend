using Streamers.Features.Categories.Features.EditCategory;
using Streamers.Features.Shared.Exceptions;
using ValidationException = Streamers.Features.Shared.Cqrs.Behaviours.ValidationException;

namespace Streamers.Features.IntegrationTests.Categories;

public class EditCategoryTests : BaseIntegrationTest
{
    public EditCategoryTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Edit_ShouldFail_WhenTitleIsEmpty()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var categoryId = await CreateCategory();
        var editCommand = new EditCategory(categoryId, "", "https://test.com/image2.png")
        {
            Id = categoryId,
            Title = "",
            Image = "https://test.com/image2.png",
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => Sender.Send(editCommand));
    }

    [Fact]
    public async Task Edit_ShouldFail_WhenCategoryDoesNotExist()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var editCommand = new EditCategory(Guid.NewGuid(), "Test 2", "https://test.com/image2.png")
        {
            Id = Guid.NewGuid(),
            Title = "Test 2",
            Image = "https://test.com/image2.png",
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => Sender.Send(editCommand));
    }

    [Fact]
    public async Task Edit_ShouldFail_WhenUserIsNotAdmin()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var categoryId = await CreateCategory();

        var streamer = await CreateStreamer(); // logs in as a regular user
        CurrentUser.MakeAuthenticated(streamer.Id);
        var editCommand = new EditCategory(categoryId, "Test 2", "https://test.com/image2.png")
        {
            Id = categoryId,
            Title = "Test 2",
            Image = "https://test.com/image2.png",
        };

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => Sender.Send(editCommand));
    }

    [Fact]
    public async Task Edit_ShouldEditCategory()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var categoryId = await CreateCategory();
        var editCommand = new EditCategory(categoryId, "Test 2", "https://test.com/image2.png")
        {
            Id = categoryId,
            Title = "Test 2",
            Image = "https://test.com/image2.png",
        };

        // Act
        await Sender.Send(editCommand);

        // Assert
        var category = DbContext.Categories.FirstOrDefault(p => p.Id == categoryId);

        Assert.NotNull(category);
        Assert.Equal(editCommand.Title, category.Title);
        Assert.Equal(editCommand.Image, category.Image);
    }
}
