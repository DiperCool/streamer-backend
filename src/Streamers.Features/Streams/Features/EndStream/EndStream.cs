using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Cqrs;
using Streamers.Features.Shared.Persistance;
using Stream = Streamers.Features.Streams.Models.Stream;

namespace Streamers.Features.Streams.Features.EndStream;

public record EndStreamResponse(Guid Id);

public record EndStream(string StreamId) : IRequest<EndStreamResponse>;

public class EndStreamHandler(StreamerDbContext dbContext)
    : IRequestHandler<EndStream, EndStreamResponse>
{
    public async Task<EndStreamResponse> Handle(
        EndStream request,
        CancellationToken cancellationToken
    )
    {
        Stream? stream = await dbContext
            .Streams.Include(x => x.Streamer)
            .FirstOrDefaultAsync(
                x => x.StreamId == request.StreamId,
                cancellationToken: cancellationToken
            );
        if (stream == null)
        {
            throw new InvalidOperationException("Stream not found");
        }

        stream.Streamer.SetLive(false, null);
        stream.SetActive(false);
        dbContext.Streamers.Update(stream.Streamer);

        dbContext.Streams.Update(stream);
        await dbContext.SaveChangesAsync(cancellationToken: cancellationToken);
        return new EndStreamResponse(stream.Id);
    }
}
