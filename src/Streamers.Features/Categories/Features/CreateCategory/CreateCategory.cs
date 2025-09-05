using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Categories.Models;
using Streamers.Features.Categories.Services;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SystemRoles.Services;

namespace Streamers.Features.Categories.Features.CreateCategory;

public record CreateCategoryResponse(Guid Id);

public record CreateCategory(string Title, string Image) : IRequest<CreateCategoryResponse>;

public class CreateCategoryHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser,
    ISystemRoleService systemRoleService,
    ISlugGenerator slugGenerator
) : IRequestHandler<CreateCategory, CreateCategoryResponse>
{
    public async Task<CreateCategoryResponse> Handle(
        CreateCategory request,
        CancellationToken cancellationToken
    )
    {
        if (!await systemRoleService.HasAdministratorRole(currentUser.UserId))
        {
            throw new UnauthorizedAccessException();
        }

        Category category = new Category(
            request.Title,
            slugGenerator.GenerateSlug(request.Title),
            request.Image
        );
        await streamerDbContext.Categories.AddAsync(category, cancellationToken);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new CreateCategoryResponse(category.Id);
    }
}
