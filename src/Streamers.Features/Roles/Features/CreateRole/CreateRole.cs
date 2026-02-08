using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Models;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Roles.Features.CreateRole;

public record CreateRoleResponse(Guid Id);

public record CreateRole(
    string BroadcasterId,
    string StreamerId,
    RoleType RoleType,
    Permissions Permissions
) : IRequest<CreateRoleResponse>;

public class CreateRoleHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser,
    ICanAssignRole assignRole
) : IRequestHandler<CreateRole, CreateRoleResponse>
{
    public async Task<CreateRoleResponse> Handle(
        CreateRole request,
        CancellationToken cancellationToken
    )
    {
        Role? admin = await streamerDbContext.Roles.FirstOrDefaultAsync(
            x => x.StreamerId == currentUser.UserId && x.BroadcasterId == request.BroadcasterId,
            cancellationToken: cancellationToken
        );
        if (admin == null)
        {
            throw new UnauthorizedAccessException();
        }
        assignRole.EnsureCanAssign(admin, request.RoleType, request.Permissions);

        Streamer? broadcaster = await streamerDbContext.Streamers.FirstOrDefaultAsync(
            x => x.Id == request.BroadcasterId,
            cancellationToken: cancellationToken
        );
        if (broadcaster == null)
        {
            throw new InvalidOperationException(
                $"Could not find broadcaster with id: {currentUser.UserId}"
            );
        }
        Streamer? streamer = await streamerDbContext.Streamers.FirstOrDefaultAsync(
            x => x.Id == request.StreamerId,
            cancellationToken: cancellationToken
        );

        if (streamer == null)
        {
            throw new InvalidOperationException(
                $"Could not find streamer with id: {currentUser.UserId}"
            );
        }

        Role role = new Role(
            streamer,
            request.RoleType,
            broadcaster,
            DateTime.UtcNow,
            request.Permissions
        );
        await streamerDbContext.Roles.AddAsync(role, cancellationToken);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new CreateRoleResponse(role.Id);
    }
}
