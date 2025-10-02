using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HotChocolate.Types;
using Shared.Storage;

namespace Shared.AzureBlob;

public class AzureBlobService(string connectionString) : IStorage
{
    private readonly BlobServiceClient _client = new(connectionString);

    public async Task<string?> AddItemAsync(
        string bucket,
        string objectName,
        Stream stream,
        string? contentType = null,
        CancellationToken cancellationToken = default
    )
    {
        var container = _client.GetBlobContainerClient(bucket);
        await container.CreateIfNotExistsAsync(
            PublicAccessType.Blob,
            cancellationToken: cancellationToken
        );

        var blob = container.GetBlobClient(objectName);
        await blob.UploadAsync(
            stream,
            new BlobHttpHeaders { ContentType = contentType },
            cancellationToken: cancellationToken
        );

        return blob.Uri.ToString();
    }

    public async Task<string?> AddItemAsync(
        string bucket,
        IFile item,
        CancellationToken cancellationToken = default
    )
    {
        await using var stream = item.OpenReadStream();
        return await AddItemAsync(bucket, item.Name, stream, item.ContentType, cancellationToken);
    }

    public async Task<IEnumerable<string?>> AddItemsAsync(
        string bucket,
        IEnumerable<IFile> items,
        CancellationToken cancellationToken = default
    )
    {
        var results = new List<string?>();
        foreach (var item in items)
        {
            results.Add(await AddItemAsync(bucket, item, cancellationToken));
        }
        return results;
    }

    public async Task<string?> CreateBucketIfNotExistsAsync(
        string bucket,
        CancellationToken cancellationToken = default
    )
    {
        var container = _client.GetBlobContainerClient(bucket);
        await container.CreateIfNotExistsAsync(
            PublicAccessType.Blob,
            cancellationToken: cancellationToken
        );
        return container.Uri.ToString();
    }

    public async Task<bool> DeleteItemAsync(
        string bucket,
        string item,
        CancellationToken cancellationToken = default
    )
    {
        var container = _client.GetBlobContainerClient(bucket);
        var blob = container.GetBlobClient(item);
        var result = await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        return result.Value;
    }

    public async Task<bool> DeleteItemsAsync(
        string bucket,
        IEnumerable<string> items,
        CancellationToken cancellationToken = default
    )
    {
        var container = _client.GetBlobContainerClient(bucket);
        bool allDeleted = true;

        foreach (var item in items)
        {
            var blob = container.GetBlobClient(item);
            var result = await blob.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            if (!result.Value)
                allDeleted = false;
        }

        return allDeleted;
    }
}
