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
        var query = streamerDbContext.Tags.AsNoTracking();

        var dtoQuery = query.Select(x => new TagDto() { Id = x.Id, Title = x.Title });

        Page<TagDto> result = await dtoQuery
            .With(request.QueryContext, DefaultOrder)
            .ToPageAsync(request.Paging, cancellationToken: cancellationToken);

        return result;
    }

    private static SortDefinition<TagDto> DefaultOrder(SortDefinition<TagDto> sort) =>
        sort.IfEmpty(o => o.AddAscending(t => t.Title));
}
