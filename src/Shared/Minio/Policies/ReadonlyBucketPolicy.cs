using System.Text.Json;
using Shared.Minio.Policies;

public class ReadonlyBucketPolicy : IBucketPolicy
{
    public string Get(string bucket)
    {
        var policy = new
        {
            Version = "2012-10-17",
            Statement = new[]
            {
                new
                {
                    Sid = "",
                    Effect = "Allow",
                    Principal = "*",
                    Action = new[] { "s3:GetObject" },
                    Resource = new[] { $"arn:aws:s3:::{bucket}/*" },
                },
            },
        };

        return JsonSerializer.Serialize(
            policy,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
            }
        );
    }
}
