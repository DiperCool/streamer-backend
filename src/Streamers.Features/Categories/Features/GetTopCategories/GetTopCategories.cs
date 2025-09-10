using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Categories.Features.GetTopCategories;

public record GetTopCategories : IRequest<List<CategoryDto>>;

public class GetTopCategoriesHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetTopCategories, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(
        GetTopCategories request,
        CancellationToken cancellationToken
    )
    {
        var only = 8;
        var result = await streamerDbContext
            .Categories.AsNoTracking()
            .Where(x => x.Streams.Any())
            .OrderByDescending(x => x.Streams.Sum(s => s.CurrentViewers))
            .Select(x => new CategoryDto
            {
                Id = x.Id,
                Title = x.Title,
                Slug = x.Slug,
                Image = x.Image,
            })
            .Take(only)
            .ToListAsync(cancellationToken: cancellationToken);
        return result;
    }
}
