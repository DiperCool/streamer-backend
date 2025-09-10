using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streams.Dtos;

namespace Streamers.Features.Streams.Features.GetStreams;

public record GetStreams(
    Guid? CategoryId,
    Guid? Tag,
    List<string> Languages,
    PagingArguments Paging,
    QueryContext<StreamDto> QueryContext
) : IRequest<Page<StreamDto>>;

public class GetStreamsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetStreams, Page<StreamDto>>
{
    public async Task<Page<StreamDto>> Handle(
        GetStreams request,
        CancellationToken cancellationToken
    )
    {
        var streams = streamerDbContext.Streams.AsNoTracking();
        if (request.Tag != null)
        {
            streams = streams.Where(s => s.Tags.Any(t => t.Id == request.Tag.Value));
        }

        if (request.CategoryId != null)
        {
            streams = streams.Where(s => s.CategoryId == request.CategoryId);
        }
        if (request.Languages.Any())
        {
            streams = streams.Where(s => request.Languages.Contains(s.Language));
        }
        var result = await streams
            .Where(x => x.Active)
            .Select(x => new StreamDto
            {
                Id = x.Id,
                Title = x.Title,
                Preview = x.Preview,
                StreamerId = x.StreamerId,
                CategoryId = x.CategoryId,
                Language = x.Language,
                Active = x.Active,
                CurrentViewers = x.CurrentViewers,
                Started = x.Started,
            })
            .With(request.QueryContext)
            .ToPageAsync(request.Paging, cancellationToken: cancellationToken);
        return result;
    }
}
