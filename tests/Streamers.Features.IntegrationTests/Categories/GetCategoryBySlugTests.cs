using System;
using System.Linq;
using System.Threading.Tasks;
using Streamers.Features.Categories.Features.GetCategoryBySlug;

namespace Streamers.Features.IntegrationTests.Categories;

public class GetCategoryBySlugTests : BaseIntegrationTest
{
    public GetCategoryBySlugTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Get_ShouldGetCategoryBySlug()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var categoryId = await CreateCategory();
        var createdCategory = DbContext.Categories.First(p => p.Id == categoryId);
        var getCommand = new GetCategoryBySlug(createdCategory.Slug);

        // Act
        var category = await Sender.Send(getCommand);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(createdCategory.Slug, category.Slug);
    }
}
