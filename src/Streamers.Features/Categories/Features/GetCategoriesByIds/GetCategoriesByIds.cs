using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Categories.Features.GetCategoriesByIds;

public record GetCategoriesByIdsResponse(IDictionary<Guid, CategoryDto> Categories);

public record GetCategoriesByIds(IReadOnlyList<Guid> Ids) : IRequest<GetCategoriesByIdsResponse>;

public class GetStreamersByIdsHandler(StreamerDbContext context)
    : IRequestHandler<GetCategoriesByIds, GetCategoriesByIdsResponse>
{
    public async Task<GetCategoriesByIdsResponse> Handle(
        GetCategoriesByIds request,
        CancellationToken cancellationToken
    )
    {
        var streamers = await context
            .Categories.Where(s => request.Ids.Contains(s.Id))
            .Select(s => new CategoryDto
            {
                Id = s.Id,
                Title = s.Title,
                Slug = s.Slug,
                Image = s.Image,
            })
            .ToListAsync(cancellationToken);

        var dict = streamers.ToDictionary(s => s.Id, s => s);

        return new GetCategoriesByIdsResponse(dict);
    }
}
