using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;
using Streamers.Features.Vods.Dtos;

namespace Streamers.Features.Vods.GraphQl;

[ObjectType<VodDto>]
public static partial class VodType
{
    public static async Task<StreamerDto?> GetStreamerAsync(
        [Parent(nameof(VodDto.StreamerId))] VodDto vod,
        IStreamersByIdDataLoader loader,
        CancellationToken cancellationToken
    )
    {
        var streamer = await loader.LoadAsync(vod.StreamerId, cancellationToken);
        return streamer;
    }
}
