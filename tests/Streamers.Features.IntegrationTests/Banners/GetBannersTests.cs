using System.Threading.Tasks;
using Streamers.Features.Banners.Features.CreateBanner;
using Streamers.Features.Banners.Features.GetBanners;

namespace Streamers.Features.IntegrationTests.Banners;

public class GetBannersTests : BaseIntegrationTest
{
    public GetBannersTests(StreamerWebApplicationFactory factory)
        : base(factory) { }

    [Fact]
    public async Task Get_ShouldReturnEmptyList_WhenStreamerHasNoBanners()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var query = new GetBanners(admin.Id);

        // Act
        var banners = await Sender.Send(query);

        // Assert
        Assert.NotNull(banners);
        Assert.Empty(banners);
    }

    [Fact]
    public async Task Get_ShouldGetBanners()
    {
        // Arrange
        var admin = await CreateAdmin();
        CurrentUser.MakeAuthenticated(admin.Id);
        var createCommand1 = new CreateBanner(
            admin.Id,
            "Banner 1",
            "Description 1",
            "https://test.com/banner1.png",
            "https://test.com/link1"
        );
        var createCommand2 = new CreateBanner(
            admin.Id,
            "Banner 2",
            "Description 2",
            "https://test.com/banner2.png",
            "https://test.com/link2"
        );
        await Sender.Send(createCommand1);
        await Sender.Send(createCommand2);

        var query = new GetBanners(admin.Id);

        // Act
        var banners = await Sender.Send(query);

        // Assert
        Assert.NotNull(banners);
        Assert.Equal(2, banners.Count);
    }
}
