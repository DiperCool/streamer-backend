using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate.Types;
using NSubstitute;
using Shared.S3;
using Shared.Storage;
using Xunit;
using Streamers.Features.Files.Features;

namespace Streamers.Features.IntegrationTests.Files;

public class UploadFileTests : BaseIntegrationTest
{
    private readonly IStorage _mockStorage;

    public UploadFileTests(StreamerWebApplicationFactory factory)
        : base(factory)
    {
        _mockStorage = factory.MockStorage;
    }

    [Fact]
    public async Task UploadFile_ShouldUploadFileAndReturnUrl()
    {
        // Arrange
        var fileName = "testfile.txt";
        var fileContent = "This is a test file.";
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));
        var expectedUrl = "https://testbucket.s3.amazonaws.com/files/testguid";

        var mockFile = Substitute.For<IFile>();
        mockFile.Name.Returns(fileName);
        mockFile.OpenReadStream().Returns(fileStream);

        _mockStorage.AddItemAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<Stream>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>()
        ).Returns(expectedUrl);

        var command = new UploadFile(mockFile);

        // Act
        var response = await Sender.Send(command);

        // Assert
        response.Should().NotBeNull();
        response.FileName.Should().Be(expectedUrl);

        await _mockStorage.Received(1).AddItemAsync(
            Arg.Any<string>(),
            Arg.Is<string>(s => s.StartsWith("files/")),
            Arg.Any<Stream>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task UploadFile_ShouldThrowExceptionWhenUploadFails()
    {
        // Arrange
        var fileName = "testfile.txt";
        var fileContent = "This is a test file.";
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        var mockFile = Substitute.For<IFile>();
        mockFile.Name.Returns(fileName);
        mockFile.OpenReadStream().Returns(fileStream);

        _mockStorage.AddItemAsync(
            Arg.Any<string>(),
            Arg.Any<string>(),
            Arg.Any<Stream>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>()
        ).Returns((string?)null); // Simulate upload failure

        var command = new UploadFile(mockFile);

        // Act
        var act = async () => await Sender.Send(command);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Couldn't upload file");
    }
}
