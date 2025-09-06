using GreenDonut.Data;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Vods.Dtos;
using Streamers.Features.Vods.Enums;

namespace Streamers.Features.Vods.Features.GetVods;

public record GetVods(string StreamerId, PagingArguments Paging, QueryContext<VodDto> QueryContext)
    : IRequest<Page<VodDto>>;

public class GetVodsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetVods, Page<VodDto>>
{
    public async Task<Page<VodDto>> Handle(GetVods request, CancellationToken cancellationToken)
    {
        Page<VodDto> result = await streamerDbContext
            .Vods.Where(x => x.StreamerId == request.StreamerId && x.Status == VodStatus.Finished)
            .Select(x => new VodDto
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Source = x.Source,
                Title = x.Title,
                Preview = x.Preview,
                Description = x.Description,
                Views = x.Views,
                StreamerId = x.StreamerId,
                Duration = x.Duration,
                CategoryId = x.CategoryId,
                Language = x.Language,
            })
            .With(request.QueryContext)
            .ToPageAsync(request.Paging, cancellationToken: cancellationToken);
        return result;
    }
}
