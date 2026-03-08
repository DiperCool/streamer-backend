using System;
using System.Linq;
using System.Threading.Tasks;
using Streamers.Features.Banners.Features.CreateBanner;
using Streamers.Features.Shared.Cqrs.Behaviours;
using Streamers.Features.Shared.Exceptions;
using ValidationException = Streamers.Features.Shared.Cqrs.Behaviours.ValidationException;

namespace Streamers.Features.IntegrationTests.Banners;

public class CreateBannerTests : BaseIntegrationTest
{
    public CreateBannerTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Create_ShouldFail_WhenTitleIsTooLong()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var longTitle = new string('a', 101);
        var command = new CreateBanner(
            admin.Id,
            longTitle,
            "Description 1",
            "https://test.com/banner.png",
            "https://test.com/link"
        );

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => Sender.Send(command));
    }

    [Fact]
    public async Task Create_ShouldFail_WhenImageIsInvalidUri()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var command = new CreateBanner(
            admin.Id,
            "Test Banner",
            "Description 1",
            "><",
            "https://test.com/link"
        );

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => Sender.Send(command));
    }

    [Fact]
    public async Task Create_ShouldFail_WhenUserIsNotAdmin()
    {
        // Arrange
        var admin = await CreateAdmin();

        var streamer = await CreateStreamer();
        CurrentUser.MakeAuthenticated(streamer.Id);

        var command = new CreateBanner(
            admin.Id,
            "Test Banner",
            "Description 1",
            "https://test.com/banner.png",
            "https://test.com/link"
        );
        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() => Sender.Send(command));
    }

    [Fact]
    public async Task Create_ShouldCreateBanner()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var command = new CreateBanner(
            admin.Id,
            "Test Banner",
            "Description 1",
            "https://test.com/banner.png",
            "https://test.com/link"
        );

        // Act
        var bannerId = await Sender.Send(command);

        // Assert
        var banner = DbContext.Banners.FirstOrDefault(b => b.Id == bannerId.Id);
        Assert.NotNull(banner);
        Assert.Equal(command.Title, banner.Title);
        Assert.Equal(command.Description, banner.Description);
        Assert.Equal(command.Image, banner.Image);
        Assert.Equal(command.Url, banner.Url);
        Assert.Equal(admin.Id, banner.StreamerId);
    }
}
