using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Profiles.Features.UpdateBio;

public record UpdateBioResponse(Guid Id);

public record UpdateBio(string Bio) : IRequest<UpdateBioResponse>;

public class UpdateBioValidator : AbstractValidator<UpdateBio>
{
    public UpdateBioValidator()
    {
        RuleFor(x => x.Bio)
            .MaximumLength(300)
            .WithMessage(x => "Bio length must be less than 300 characters.");
    }
}

public class UpdateBioHandler(StreamerDbContext context, ICurrentUser currentUser)
    : IRequestHandler<UpdateBio, UpdateBioResponse>
{
    public async Task<UpdateBioResponse> Handle(
        UpdateBio request,
        CancellationToken cancellationToken
    )
    {
        var profile = await context.Profiles.FirstOrDefaultAsync(
            p => p.StreamerId == currentUser.UserId,
            cancellationToken
        );

        if (profile == null)
        {
            throw new Exception($"Profile for user {currentUser.UserId} not found");
        }

        profile.Bio = request.Bio;

        await context.SaveChangesAsync(cancellationToken);

        return new UpdateBioResponse(profile.Id);
    }
}
