using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults;
using Streamers.Features.Notifications.Services;
using Streamers.Features.Shared.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Models;
using Streamers.Features.Streamers.Services;
using Streamers.Features.Streams.Enums;
using Streamers.Features.Streams.Models;
using Streamers.Features.Vods.Models;
using Stream = Streamers.Features.Streams.Models.Stream;

namespace Streamers.Features.Streams.Features.CreateStream;

public record CreateStreamResponse(Guid Id);

// todo: fix transaction
public record CreateStream(string StreamName, string StreamId) : IRequest<CreateStreamResponse>;

public class CreateStreamHandler(StreamerDbContext dbContext, IConfiguration configuration)
    : IRequestHandler<CreateStream, CreateStreamResponse>
{
    public async Task<CreateStreamResponse> Handle(
        CreateStream request,
        CancellationToken cancellationToken
    )
    {
        var opts = configuration.BindOptions<RtmpOptions>();
        Streamer? streamer = await dbContext
            .Streamers.Include(x => x.StreamInfo)
            .ThenInclude(streamInfo => streamInfo.Tags)
            .Include(streamer => streamer.Profile)
            .Include(streamer => streamer.StreamInfo)
            .ThenInclude(streamInfo => streamInfo.Category)
            .Include(x => x.VodSettings)
            .FirstOrDefaultAsync(
                x => x.StreamSettings.StreamName == request.StreamName,
                cancellationToken: cancellationToken
            );

        if (streamer == null)
        {
            return new CreateStreamResponse(Guid.NewGuid());
        }
        List<StreamSource> sources = new()
        {
            new StreamSource
            {
                SourceType = StreamSourceType.Hls,
                Url = $"{opts.Hls}/{request.StreamName}/index.m3u8",
            },
            new StreamSource
            {
                SourceType = StreamSourceType.WebRtc,
                Url = $"{opts.WebRtc}/{request.StreamName}/whep",
            },
        };
        var info = streamer.StreamInfo;
        Stream stream = new Stream(
            streamer,
            request.StreamId,
            info.Title ?? string.Empty,
            DateTime.UtcNow,
            sources,
            info.Language,
            info.Tags,
            streamer.Profile.ChannelBanner,
            info.Category
        );
        streamer.SetLive(true, stream);
        if (streamer.VodSettings.VodEnabled)
        {
            var processVodUrl = $"{opts.VodProcess}/{request.StreamName}";

            Vod vod = new Vod(
                Guid.NewGuid(),
                stream.Streamer,
                DateTime.UtcNow,
                processVodUrl,
                info.Language
            );
            await dbContext.Vods.AddAsync(vod, cancellationToken);
        }

        await dbContext.Streams.AddAsync(stream, cancellationToken);
        dbContext.Streamers.Update(streamer);

        await dbContext.SaveChangesAsync(cancellationToken);
        return new CreateStreamResponse(stream.Id);
    }
}
