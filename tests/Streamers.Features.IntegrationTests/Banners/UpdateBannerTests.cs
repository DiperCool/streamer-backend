using System;
using System.Linq;
using System.Threading.Tasks;
using Streamers.Features.Banners.Exceptions;
using Streamers.Features.Banners.Features.CreateBanner;
using Streamers.Features.Banners.Features.UpdateBanner;
using Streamers.Features.Shared.Exceptions;
using ValidationException = Streamers.Features.Shared.Cqrs.Behaviours.ValidationException;

namespace Streamers.Features.IntegrationTests.Banners;

public class UpdateBannerTests : BaseIntegrationTest
{
    public UpdateBannerTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Update_ShouldFail_WhenTitleIsTooLong()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var createCommand = new CreateBanner(
            admin.Id,
            "Test Banner",
            "Description 1",
            "https://test.com/banner.png",
            "https://test.com/link"
        );
        var bannerId = await Sender.Send(createCommand);

        var longTitle = new string('a', 101);
        var updateCommand = new UpdateBanner(
            admin.Id,
            bannerId.Id,
            longTitle,
            "Updated Description",
            "https://updated.com/banner.png",
            "https://updated.com/link"
        );

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => Sender.Send(updateCommand));
    }

    [Fact]
    public async Task Update_ShouldFail_WhenBannerDoesNotExist()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var updateCommand = new UpdateBanner(
            admin.Id,
            Guid.NewGuid(),
            "Updated Banner",
            "Updated Description",
            "https://updated.com/banner.png",
            "https://updated.com/link"
        );

        // Act & Assert
        await Assert.ThrowsAsync<BannerNotFoundException>(() => Sender.Send(updateCommand));
    }

    [Fact]
    public async Task Update_ShouldFail_WhenUserIsNotAdmin()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var createCommand = new CreateBanner(
            admin.Id,
            "Test Banner",
            "Description 1",
            "https://test.com/banner.png",
            "https://test.com/link"
        );
        var bannerId = await Sender.Send(createCommand);

        var regularUser = await CreateStreamer();
        CurrentUser.MakeAuthenticated(regularUser.Id);
        var updateCommand = new UpdateBanner(
            admin.Id,
            bannerId.Id,
            "Updated Banner",
            "Updated Description",
            "https://updated.com/banner.png",
            "https://updated.com/link"
        );

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => Sender.Send(updateCommand));
    }

    [Fact]
    public async Task Update_ShouldUpdateBanner()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var createCommand = new CreateBanner(
            admin.Id,
            "Test Banner",
            "Description 1",
            "https://test.com/banner.png",
            "https://test.com/link"
        );
        var bannerId = await Sender.Send(createCommand);
        var updateCommand = new UpdateBanner(
            admin.Id,
            bannerId.Id,
            "Updated Banner",
            "Updated Description",
            "https://updated.com/banner.png",
            "https://updated.com/link"
        );

        // Act
        await Sender.Send(updateCommand);

        // Assert
        var banner = DbContext.Banners.FirstOrDefault(b => b.Id == bannerId.Id);
        Assert.NotNull(banner);
        Assert.Equal(updateCommand.Title, banner.Title);
        Assert.Equal(updateCommand.Description, banner.Description);
        Assert.Equal(updateCommand.Image, banner.Image);
        Assert.Equal(updateCommand.Url, banner.Url);
    }
}
