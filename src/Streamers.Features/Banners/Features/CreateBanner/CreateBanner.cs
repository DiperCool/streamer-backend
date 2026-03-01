using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Banners.Model;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Exceptions;

namespace Streamers.Features.Banners.Features.CreateBanner;

public record CreateBannerResponse(Guid Id);

public record CreateBanner(
    string StreamerId,
    string? Title,
    string? Description,
    string? Image,
    string? Url
) : IRequest<CreateBannerResponse>;

public class CreateBannerValidator : AbstractValidator<CreateBanner>
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

public class CreateBannerHandle(
    StreamerDbContext streamerDbContext,
    IRoleService roleService,
    ICurrentUser currentUser
) : IRequestHandler<CreateBanner, CreateBannerResponse>
{
    public async Task<CreateBannerResponse> Handle(
        CreateBanner request,
        CancellationToken cancellationToken
    )
    {
        if (!await roleService.HasRole(request.StreamerId, currentUser.UserId, Permissions.Banners))
        {
            throw new ForbiddenException();
        }
        var broadcaster = await streamerDbContext.Streamers.FirstOrDefaultAsync(
            x => x.Id == request.StreamerId,
            cancellationToken: cancellationToken
        );
        if (broadcaster == null)
        {
            throw new StreamerNotFoundException(request.StreamerId);
        }
        var banner = new Banner(
            request.Title,
            request.Description,
            request.Image,
            request.Url,
            broadcaster
        );
        await streamerDbContext.Banners.AddAsync(banner, cancellationToken);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new CreateBannerResponse(banner.Id);
    }
}
