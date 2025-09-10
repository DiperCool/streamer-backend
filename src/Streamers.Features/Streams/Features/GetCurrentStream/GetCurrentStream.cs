using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streams.Dtos;

namespace Streamers.Features.Streams.Features.GetCurrentStream;

public record GetCurrentStream(string StreamerId) : IRequest<StreamDto>;

public class GetCurrentStreamHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetCurrentStream, StreamDto>
{
    public async Task<StreamDto> Handle(
        GetCurrentStream request,
        CancellationToken cancellationToken
    )
    {
        var stream = await streamerDbContext
            .Streamers.Where(x => x.Id == request.StreamerId)
            .Select(x => x.CurrentStream)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (stream == null)
        {
            throw new InvalidOperationException("Stream not found");
        }

        return new StreamDto
        {
            Id = stream.Id,
            StreamerId = stream.StreamerId,
            Active = stream.Active,
            Title = stream.Title,
            CurrentViewers = stream.CurrentViewers,
            Language = stream.Language,
            CategoryId = stream.CategoryId,
            Started = stream.Started,
            Preview = stream.Preview,
        };
    }
}
