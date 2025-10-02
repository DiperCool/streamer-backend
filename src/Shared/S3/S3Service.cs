using Amazon.S3;
using Amazon.S3.Model;
using HotChocolate.Types;
using Microsoft.Extensions.Configuration;
using Shared.Storage;
using streamer.ServiceDefaults;

namespace Shared.S3;

public class S3Service(IAmazonS3 s3Client, IConfiguration configuration) : IStorage
{
    private readonly S3Options _options = configuration.BindOptions<S3Options>();

    public async Task<string?> AddItemAsync(
        string bucket,
        string objectName,
        Stream stream,
        string? contentType = null,
        CancellationToken cancellationToken = default
    )
    {
        var request = new PutObjectRequest
        {
            BucketName = bucket,
            Key = objectName,
            InputStream = stream,
            ContentType = contentType ?? "application/octet-stream",
            CannedACL = S3CannedACL.PublicRead,
            Headers = { ["Cache-Control"] = "public, max-age=31536000" },
        };

        var response = await s3Client.PutObjectAsync(request, cancellationToken);

        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            return objectName;
        }

        return null;
    }

    public async Task<string?> AddItemAsync(
        string bucket,
        IFile item,
        CancellationToken cancellationToken = default
    )
    {
        await using var stream = item.OpenReadStream();
        return await AddItemAsync(
            bucket,
            Guid.NewGuid().ToString(),
            stream,
            item.ContentType,
            cancellationToken
        );
    }

    // Загружаем несколько IFile
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

    // Создаём бакет, если его нет
    public async Task<string?> CreateBucketIfNotExistsAsync(
        string bucket,
        CancellationToken cancellationToken = default
    )
    {
        if (!await BucketExistsAsync(bucket))
        {
            await s3Client.PutBucketAsync(bucket, cancellationToken);
        }
        return bucket;
    }

    // Проверяем существование бакета
    private async Task<bool> BucketExistsAsync(string bucket)
    {
        try
        {
            var response = await s3Client.ListBucketsAsync();
            return response.Buckets.Any(b => b.BucketName == bucket);
        }
        catch
        {
            return false;
        }
    }

    // Удаляем один объект
    public async Task<bool> DeleteItemAsync(
        string bucket,
        string item,
        CancellationToken cancellationToken = default
    )
    {
        var response = await s3Client.DeleteObjectAsync(bucket, item, cancellationToken);
        return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent
            || response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    // Удаляем несколько объектов
    public async Task<bool> DeleteItemsAsync(
        string bucket,
        IEnumerable<string> items,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var item in items)
        {
            await DeleteItemAsync(bucket, item, cancellationToken);
        }
        return true;
    }
}
