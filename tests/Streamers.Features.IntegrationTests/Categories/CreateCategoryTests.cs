using Streamers.Features.Categories.Features.CreateCategory;
using ValidationException = Streamers.Features.Shared.Cqrs.Behaviours.ValidationException;

namespace Streamers.Features.IntegrationTests.Categories;

public class CreateCategoryTests : BaseIntegrationTest
{
    public CreateCategoryTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Create_ShouldFail_WhenTitleIsEmpty()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var command = new CreateCategory("", "https://test.com/image.png") { Title = "", Image = "https://test.com/image.png" };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => Sender.Send(command));
    }

    [Fact]
    public async Task Create_ShouldFail_WhenUserIsNotAdmin()
    {
        // Arrange
        var streamer = await CreateStreamer();
        CurrentUser.MakeAuthenticated(streamer.Id);
        var command = new CreateCategory("Test", "https://test.com/image.png") { Title = "Test", Image = "https://test.com/image.png" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => Sender.Send(command));
    }

    [Fact]
    public async Task Create_ShouldCreateCategory()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var command = new CreateCategory("Test", "https://test.com/image.png") { Title = "Test", Image = "https://test.com/image.png" };

        // Act
        var categoryId = await Sender.Send(command);

        // Assert
        var category = DbContext.Categories.FirstOrDefault(p => p.Id == categoryId.Id);

        Assert.NotNull(category);
        Assert.Equal(command.Title, category.Title);
        Assert.Equal(command.Image, category.Image);
    }
}