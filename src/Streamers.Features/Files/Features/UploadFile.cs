using HotChocolate.Types;
using Microsoft.Extensions.Configuration;
using Shared.Abstractions.Cqrs;
using Shared.S3;
using Shared.Storage;
using streamer.ServiceDefaults;

namespace Streamers.Features.Files.Features;

public record UploadFileResponse(string FileName);

public record UploadFile(IFile File) : IRequest<UploadFileResponse>;

public class UploadFileHandler(IStorage minioServices, IConfiguration configuration)
    : IRequestHandler<UploadFile, UploadFileResponse>
{
    public async Task<UploadFileResponse> Handle(
        UploadFile request,
        CancellationToken cancellationToken
    )
    {
        var opts = configuration.BindOptions<S3Options>();

        await using var stream = request.File.OpenReadStream();

        var url = await minioServices.AddItemAsync(
            opts.Bucket,
            $"files/{Guid.NewGuid()}",
            stream,
            null,
            cancellationToken
        );
        if (url == null)
        {
            throw new InvalidOperationException("Couldn't upload file");
        }
        return new UploadFileResponse(url);
    }
}
