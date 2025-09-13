using FluentValidation;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Chats.Models;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.BanUser;

public record BanUserResponse(Guid Id);

public record BanUser(string BroadcasterId, string UserId, DateTime BanUntil, string Reason)
    : IRequest<BanUserResponse>;

public class BanUserValidator : AbstractValidator<BanUser>
{
    public BanUserValidator()
    {
        RuleFor(x => x.BroadcasterId).NotEmpty().WithMessage("BroadcasterId is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("UserId is required.")
            .NotEqual(x => x.BroadcasterId)
            .WithMessage("User cannot ban themselves.");

        RuleFor(x => x.BanUntil)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("BanUntil must be in the future.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required.")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters.");
    }
}

public class BanUserHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<BanUser, BanUserResponse>
{
    public async Task<BanUserResponse> Handle(BanUser request, CancellationToken cancellationToken)
    {
        var role = await streamerDbContext
            .Roles.Include(x => x.Streamer)
            .FirstOrDefaultAsync(
                x => x.StreamerId == currentUser.UserId,
                cancellationToken: cancellationToken
            );

        if (role == null || !role.Permissions.HasPermission(Permissions.Chat))
        {
            throw new InvalidOperationException("You do not have permission to use this command");
        }
        if (
            await streamerDbContext.BannedUsers.AnyAsync(
                x => x.UserId == request.UserId && x.BroadcasterId == request.BroadcasterId,
                cancellationToken: cancellationToken
            )
        )
        {
            throw new InvalidOperationException("Already banned user");
        }

        var broadcaster = await streamerDbContext.Streamers.FirstOrDefaultAsync(
            x => x.Id == request.BroadcasterId,
            cancellationToken
        );
        if (broadcaster == null)
        {
            throw new InvalidOperationException("Broadcaster not found");
        }
        var user = await streamerDbContext.Streamers.FirstOrDefaultAsync(
            x => x.Id == request.UserId,
            cancellationToken
        );
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }
        BannedUser bannedUser = new BannedUser(
            user,
            broadcaster,
            role.Streamer,
            DateTime.UtcNow,
            request.BanUntil,
            request.Reason
        );
        var jobId = BackgroundJob.Schedule<IMediator>(
            mediator => mediator.Send(new ClearBan.ClearBan(bannedUser.Id), CancellationToken.None),
            bannedUser.BannedUntil
        );
        bannedUser.JobId = jobId;
        streamerDbContext.BannedUsers.Add(bannedUser);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new BanUserResponse(bannedUser.Id);
    }
}
