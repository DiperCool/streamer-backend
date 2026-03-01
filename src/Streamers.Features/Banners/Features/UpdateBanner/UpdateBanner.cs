using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Banners.Exceptions; // New using statement

namespace Streamers.Features.Banners.Features.UpdateBanner;

public record UpdateBannerResponse(Guid Id);

public record UpdateBanner(
    string StreamerId,
    Guid BannerId,
    string? Title,
    string? Description,
    string? Image,
    string? Url
) : IRequest<UpdateBannerResponse>;

public class CreateBannerValidator : AbstractValidator<UpdateBanner>
{
    public CreateBannerValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(100)
            .WithMessage("Title must be at most 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must be at most 500 characters.");

        RuleFor(x => x.Image)
            .Must(uri =>
                string.IsNullOrWhiteSpace(uri)
                || Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute)
            )
            .WithMessage("Image must be a valid URI if provided.");

        RuleFor(x => x.Url)
            .Must(uri =>
                string.IsNullOrWhiteSpace(uri) || Uri.IsWellFormedUriString(uri, UriKind.Absolute)
            )
            .WithMessage("Url must be a valid absolute URI if provided.");
    }
}

public class UpdateBannerHandler(
    StreamerDbContext streamerDbContext,
    IRoleService roleService,
    ICurrentUser currentUser
) : IRequestHandler<UpdateBanner, UpdateBannerResponse>
{
    public async Task<UpdateBannerResponse> Handle(
        UpdateBanner request,
        CancellationToken cancellationToken
    )
    {
        if (!await roleService.HasRole(request.StreamerId, currentUser.UserId, Permissions.Banners))
        {
            throw new ForbiddenException();
        }

        var banner = await streamerDbContext.Banners.FirstOrDefaultAsync(
            x => x.Id == request.BannerId,
            cancellationToken: cancellationToken
        );
        if (banner == null)
        {
            throw new BannerNotFoundException(request.BannerId);
        }
        banner.Update(request.Title, request.Description, request.Image, request.Url);
        streamerDbContext.Banners.Update(banner);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new UpdateBannerResponse(banner.Id);
    }
}
