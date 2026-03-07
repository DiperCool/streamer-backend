using FluentValidation;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Categories.Models;
using Streamers.Features.Categories.Services;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SystemRoles.Services;

namespace Streamers.Features.Categories.Features.CreateCategory;

public record CreateCategoryResponse(Guid Id);

public record CreateCategory(string Title, string Image) : IRequest<CreateCategoryResponse>
{
    public required string Title { get; init; } = Title;
    public required string Image { get; init; } = Image;
}

public class CreateCategoryValidator : AbstractValidator<CreateCategory>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Image)
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("Image must be a valid absolute URI.");
    }
}

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
