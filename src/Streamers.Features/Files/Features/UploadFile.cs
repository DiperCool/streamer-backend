using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Shared.Minio;

namespace Streamers.Features.Files.Features;

public record UploadFileResponse(string FileName);

public record UploadFile(IFile File) : IRequest<UploadFileResponse>;

public class UploadFileHandler(IMinioService minioServices)
    : IRequestHandler<UploadFile, UploadFileResponse>
{
    public async Task<UploadFileResponse> Handle(
        UploadFile request,
        CancellationToken cancellationToken
    )
    {
        var fileName = await minioServices.AddItemAsync(
            Images.Bucket,
            request.File,
            cancellationToken
        );
        if (string.IsNullOrEmpty(fileName))
        {
            throw new NullReferenceException("The file could not be added");
        }
        return new UploadFileResponse(fileName);
    }
}
