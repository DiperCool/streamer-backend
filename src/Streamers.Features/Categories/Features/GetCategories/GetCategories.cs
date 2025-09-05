using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Categories.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Categories.Features.GetCategories;

public record GetCategories(
    string? Search,
    PagingArguments Paging,
    QueryContext<CategoryDto> QueryContext
) : IRequest<Page<CategoryDto>>;

public class GetCategoriesHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetCategories, Page<CategoryDto>>
{
    public async Task<Page<CategoryDto>> Handle(
        GetCategories request,
        CancellationToken cancellationToken
    )
    {
        var query = streamerDbContext.Categories.AsNoTracking();

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(x => EF.Functions.ILike(x.Title, $"%{request.Search}%"));
        }

        var dtoQuery = query.Select(x => new CategoryDto
        {
            Id = x.Id,
            Title = x.Title,
            Slug = x.Slug,
            Image = x.Image,
        });

        Page<CategoryDto> result = await dtoQuery
            .With(request.QueryContext)
            .ToPageAsync(request.Paging, cancellationToken: cancellationToken);

        return result;
    }
}
