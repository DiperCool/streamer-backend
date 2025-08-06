using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using Shared.Minio.Policies;

namespace Shared.Minio;

public interface IMinioService
{
    Task<string?> AddItemAsync(
        string bucket,
        string objectName,
        Stream stream,
        string? contentType = null,
        CancellationToken cancellationToken = default
    );
    Task<string?> AddItemAsync(
        string bucket,
        IFile item,
        CancellationToken cancellationToken = default
    );
    Task<IEnumerable<string?>> AddItemsAsync(
        string bucket,
        IEnumerable<IFile> items,
        CancellationToken cancellationToken = default
    );

    Task<string?> CreateBucketIfNotExistsAsync(
        string bucket,
        IBucketPolicy? policy,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteItemAsync(
        string bucket,
        string item,
        CancellationToken cancellationToken = default
    );
    Task<bool> DeleteItemsAsync(
        string bucket,
        IEnumerable<string> items,
        CancellationToken cancellationToken = default
    );
}
