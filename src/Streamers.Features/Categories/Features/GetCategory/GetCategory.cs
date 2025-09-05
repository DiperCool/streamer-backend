using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Categories.Features.GetCategory;

public record GetCategory(Guid Id) : IRequest<CategoryDto>;

public class GetCategoryHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetCategory, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategory request, CancellationToken cancellationToken)
    {
        var category = await streamerDbContext.Categories.FirstOrDefaultAsync(
            x => x.Id == request.Id,
            cancellationToken
        );
        if (category == null)
        {
            throw new InvalidOperationException($"No category found with id {request.Id}");
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
