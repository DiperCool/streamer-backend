using Streamers.Features.Streams.Models;

namespace Streamers.Features.Streamers.Services;

public interface IStreamKeyGenerator
{
    void GenerateKey(StreamSettings streamSettings);
}
