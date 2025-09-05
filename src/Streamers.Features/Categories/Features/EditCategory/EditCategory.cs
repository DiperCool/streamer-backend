using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Categories.Services;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SystemRoles.Services;

namespace Streamers.Features.Categories.Features.EditCategory;

public record EditCategoryResponse(Guid Id);

public record EditCategory(Guid Id, string Title, string Image) : IRequest<EditCategoryResponse>;

public class EditCategoryHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser,
    ISystemRoleService systemRoleService,
    ISlugGenerator slugGenerator
) : IRequestHandler<EditCategory, EditCategoryResponse>
{
    public async Task<EditCategoryResponse> Handle(
        EditCategory request,
        CancellationToken cancellationToken
    )
    {
        if (!await systemRoleService.HasAdministratorRole(currentUser.UserId))
        {
            throw new UnauthorizedAccessException();
        }
        var category = await streamerDbContext.Categories.FirstOrDefaultAsync(x =>
            x.Id == request.Id
        );
        if (category == null)
        {
            throw new InvalidOperationException();
        }
        category.Title = request.Title;
        category.Image = request.Image;
        category.Slug = slugGenerator.GenerateSlug(request.Title);
        streamerDbContext.Categories.Update(category);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new EditCategoryResponse(request.Id);
    }
}
