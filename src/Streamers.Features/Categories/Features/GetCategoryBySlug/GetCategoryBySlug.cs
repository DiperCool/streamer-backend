using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Categories.Features.GetCategoryBySlug;

public record GetCategoryBySlug(string Slug) : IRequest<CategoryDto>;

public class GetCategoryBySlugHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetCategoryBySlug, CategoryDto>
{
    public async Task<CategoryDto> Handle(
        GetCategoryBySlug request,
        CancellationToken cancellationToken
    )
    {
        var category = await streamerDbContext.Categories.FirstOrDefaultAsync(x =>
            x.Slug == request.Slug
        );
        if (category == null)
        {
            throw new InvalidOperationException($"No category found with slug {request.Slug}");
        }
        return new CategoryDto
        {
            Id = category.Id,
            Title = category.Title,
            Slug = category.Slug,
            Image = category.Image,
        };
    }
}
