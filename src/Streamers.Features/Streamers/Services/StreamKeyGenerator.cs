using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Streams.Models;

namespace Streamers.Features.Streamers.Services;

public class RtmpOptions
{
    public string Host { get; set; }
    public string App { get; set; }
    public string Hls { get; set; }
    public string WebRtc { get; set; }
    public string VodProcess { get; set; }
    public string Scheme { get; set; }
}

public class StreamKeyGenerator(IConfiguration configuration) : IStreamKeyGenerator
{
    private const int KeyLengthInBytes = 16;

    public void GenerateKey(StreamSettings streamSettings)
    {
        using var rng = RandomNumberGenerator.Create();
        byte[] keyBytes = new byte[KeyLengthInBytes];
        rng.GetBytes(keyBytes);

        string token = Convert.ToBase64String(keyBytes).Replace("/", "_").Replace("+", "-");
        var streamId = Guid.NewGuid();
        var options =
            configuration.GetSection("RtmpOptions").Get<RtmpOptions>()
            ?? throw new InvalidOperationException("RtmpOptions not found in configuration.");
        streamSettings.StreamId = streamId;

        streamSettings.StreamName = $"{options.App}/{streamSettings.StreamId}";
        var url = $"{options.Scheme}://{options.Host}/{options.App}";
        streamSettings.StreamUrl = url;

        streamSettings.StreamKeyToken = token;
        streamSettings.StreamKey = $"{streamSettings.StreamId}?token={token}";
    }
}
