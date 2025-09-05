using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SystemRoles.Services;

namespace Streamers.Features.Categories.Features.RemoveCategory;

public record RemoveCategoryResponse(Guid Id);

public record RemoveCategory(Guid Id) : IRequest<RemoveCategoryResponse>;

public class RemoveCategoryHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser,
    ISystemRoleService systemRoleService
) : IRequestHandler<RemoveCategory, RemoveCategoryResponse>
{
    public async Task<RemoveCategoryResponse> Handle(
        RemoveCategory request,
        CancellationToken cancellationToken
    )
    {
        if (!await systemRoleService.HasAdministratorRole(currentUser.UserId))
        {
            throw new UnauthorizedAccessException();
        }
        var category = await streamerDbContext.Categories.FirstOrDefaultAsync(
            x => x.Id == request.Id,
            cancellationToken: cancellationToken
        );
        if (category == null)
        {
            throw new InvalidOperationException();
        }
        streamerDbContext.Remove(category);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new RemoveCategoryResponse(category.Id);
    }
}
