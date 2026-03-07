using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Categories.Services;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SystemRoles.Services;

namespace Streamers.Features.Categories.Features.EditCategory;

public record EditCategoryResponse(Guid Id);

public record EditCategory(Guid Id, string Title, string Image) : IRequest<EditCategoryResponse>
{
    public Guid Id { get; init; } = Id;
    public required string Title { get; init; } = Title;
    public required string Image { get; init; } = Image;
}

public class EditCategoryValidator : AbstractValidator<EditCategory>
{
    public EditCategoryValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Image)
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("Image must be a valid absolute URI.");
    }
}

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
            throw new ForbiddenException();
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
