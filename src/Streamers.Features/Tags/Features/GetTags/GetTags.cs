using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Tags.Dto;

namespace Streamers.Features.Tags.Features.GetTags;

public record GetTags(PagingArguments Paging, QueryContext<TagDto> QueryContext)
    : IRequest<Page<TagDto>>;

public class GetTagsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetTags, Page<TagDto>>
{
    public async Task<Page<TagDto>> Handle(GetTags request, CancellationToken cancellationToken)
    {
        var result = await streamerDbContext
            .Tags.AsNoTracking()
            .Select(x => new TagDto() { Id = x.Id, Title = x.Title })
            .With(request.QueryContext)
            .ToPageAsync(request.Paging, cancellationToken: cancellationToken);
        return result;
    }
}
