using Shared.Minio;
using Shared.Seeds;

namespace Streamers.Features.Files.Seeds;

public class MinioBucketSeeds(IMinioService service) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        await service.CreateBucketIfNotExistsAsync(Images.Bucket, new ReadonlyBucketPolicy());

        var defaultFilesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "default");

        if (!Directory.Exists(defaultFilesDir))
            return;

        var files = Directory.GetFiles(defaultFilesDir, "*", SearchOption.AllDirectories);

        foreach (var filePath in files)
        {
            var objectName = "default/" + Path.GetFileName(filePath);
            var contentType = GetContentType(filePath);

            await using var stream = File.OpenRead(filePath);

            await service.AddItemAsync(Images.Bucket, objectName, stream, contentType);
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
