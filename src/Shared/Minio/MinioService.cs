using System.Reactive.Linq;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel.Args;
using Shared.Minio.Policies;
using streamer.ServiceDefaults;

namespace Shared.Minio;

public class MinioService : IMinioService
{
    private readonly ILogger<MinioService> _logger;
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;

    public MinioService(
        IMinioClient minioClient,
        ILogger<MinioService> logger,
        IConfiguration configuration
    )
    {
        _minioClient = minioClient;
        _logger = logger;
        _options = configuration.BindOptions<MinioOptions>();
    }

    public async Task<string?> AddItemAsync(
        string bucket,
        string objectName,
        Stream stream,
        string? contentType = null,
        CancellationToken cancellationToken = default
    )
    {
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithHeaders(
                new Dictionary<string, string>() { { "Cache-Control", "public, max-age=31536000" } }
            )
            .WithObjectSize(stream.Length);

        if (!string.IsNullOrEmpty(contentType))
        {
            putObjectArgs.WithContentType(contentType);
        }

        await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

        return objectName;
    }

    public async Task<string?> AddItemAsync(
        string bucket,
        IFile item,
        CancellationToken cancellationToken = default
    )
    {
        Guid id = Guid.NewGuid();
        var fileExtension = Path.GetExtension(item.Name);
        var uniqueFileName = $"{id}{fileExtension}";

        await _minioClient.PutObjectAsync(
            new PutObjectArgs()
                .WithBucket(bucket)
                .WithObject(uniqueFileName)
                .WithHeaders(
                    new Dictionary<string, string>()
                    {
                        { "Cache-Control", "public, max-age=31536000" },
                    }
                )
                .WithObjectSize(item.Length ?? 0)
                .WithStreamData(item.OpenReadStream()),
            cancellationToken
        );
        _logger.LogInformation(
            "Item added successfully File = {UniqueFileName}, Bucket = {Bucket}",
            uniqueFileName,
            bucket
        );
        var fileUrl = $"{uniqueFileName}";
        return fileUrl;
    }

    public async Task<IEnumerable<string?>> AddItemsAsync(
        string bucket,
        IEnumerable<IFile> items,
        CancellationToken cancellationToken = default
    )
    {
        var tasks = items.Select(item => AddItemAsync(bucket, item, cancellationToken)).ToList();
        return await Task.WhenAll(tasks);
    }

    public async Task<string?> CreateBucketIfNotExistsAsync(
        string bucket,
        IBucketPolicy? policy,
        CancellationToken cancellationToken = default
    )
    {
        if (
            !await _minioClient.BucketExistsAsync(
                new BucketExistsArgs().WithBucket(bucket),
                cancellationToken
            )
        )
        {
            _logger.LogInformation("Bucket created with name = {Bucket}", bucket);
            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(bucket),
                cancellationToken
            );
            string policyStr = policy?.Get(bucket) ?? "";

            await _minioClient.SetPolicyAsync(
                new SetPolicyArgs().WithBucket(bucket).WithPolicy(policyStr),
                cancellationToken
            );
        }

        return bucket;
    }

    public async Task<bool> DeleteItemAsync(
        string bucket,
        string item,
        CancellationToken cancellationToken = default
    )
    {
        await _minioClient.RemoveObjectAsync(
            new RemoveObjectArgs().WithBucket(bucket).WithObject(item),
            cancellationToken
        );
        return true;
    }

    public async Task<bool> DeleteItemsAsync(
        string bucket,
        IEnumerable<string> items,
        CancellationToken cancellationToken = default
    )
    {
        var tasks = items.Select(item => DeleteItemAsync(bucket, item, cancellationToken)).ToList();
        await Task.WhenAll(tasks);
        return true;
    }
}
