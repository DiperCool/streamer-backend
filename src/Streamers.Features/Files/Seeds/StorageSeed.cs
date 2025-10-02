using Microsoft.Extensions.Configuration;
using Shared.S3;
using Shared.Seeds;
using Shared.Storage;
using streamer.ServiceDefaults;

namespace Streamers.Features.Files.Seeds;

public class MinioBucketSeeds(IStorage service, IConfiguration configuration) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        var opts = configuration.BindOptions<S3Options>();
        await service.CreateBucketIfNotExistsAsync(opts.Bucket);

        var defaultFilesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "default");

        if (!Directory.Exists(defaultFilesDir))
            return;

        var files = Directory.GetFiles(defaultFilesDir, "*", SearchOption.AllDirectories);

        foreach (var filePath in files)
        {
            var objectName = "files/default/" + Path.GetFileName(filePath);
            var contentType = GetContentType(filePath);

            await using var stream = File.OpenRead(filePath);

            await service.AddItemAsync(opts.Bucket, objectName, stream, contentType);
        }
        string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream",
            };
        }
    }

    public int Order => 1;
}
