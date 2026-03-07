using Streamers.Features.Banners.Exceptions;
using Streamers.Features.Banners.Features.CreateBanner;
using Streamers.Features.Banners.Features.RemoveBanner;
using Streamers.Features.Shared.Exceptions;

namespace Streamers.Features.IntegrationTests.Banners;

public class RemoveBannerTests : BaseIntegrationTest
{
    public RemoveBannerTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Remove_ShouldFail_WhenBannerDoesNotExist()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var removeCommand = new RemoveBanner(admin.Id, Guid.NewGuid());

        // Act & Assert
        await Assert.ThrowsAsync<BannerNotFoundException>(() => Sender.Send(removeCommand));
    }

    [Fact]
    public async Task Remove_ShouldFail_WhenUserIsNotAdmin()
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
        var removeCommand = new RemoveBanner(admin.Id, bannerId.Id);

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => Sender.Send(removeCommand));
    }

    [Fact]
    public async Task Remove_ShouldRemoveBanner()
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
        var removeCommand = new RemoveBanner(admin.Id, bannerId.Id);

        // Act
        await Sender.Send(removeCommand);

        // Assert
        var banner = DbContext.Banners.FirstOrDefault(b => b.Id == bannerId.Id);
        Assert.Null(banner);
    }
}
