using GreenDonut.Data;
using Streamers.Features.Categories.Dtos;
using System;
using System.Threading.Tasks;
using Streamers.Features.Categories.Features.GetCategories;

namespace Streamers.Features.IntegrationTests.Categories;

public class GetCategoriesTests : BaseIntegrationTest
{
    public GetCategoriesTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetList_ShouldGetCategories()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        await CreateCategory();
        var getCommand = new GetCategories(
            null,
            null,
            new PagingArguments(),
            new QueryContext<CategoryDto>()
        );

        // Act
        var categories = await Sender.Send(getCommand);

        // Assert
        Assert.NotNull(categories);
        Assert.Single(categories.Items);
    }
}
