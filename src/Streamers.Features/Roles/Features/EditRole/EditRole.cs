using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Exceptions;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Roles.Features.EditRole;

public record EditRoleResponse(Guid Id);

public record EditRole(Guid RoleId, Permissions Permissions) : IRequest<EditRoleResponse>;

public class EditRoleHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser,
    ICanAssignRole assignRole
) : IRequestHandler<EditRole, EditRoleResponse>
{
    public async Task<EditRoleResponse> Handle(
        EditRole request,
        CancellationToken cancellationToken
    )
    {
        var role = await streamerDbContext
            .Roles.Include(x => x.Broadcaster)
            .Include(x => x.Streamer)
            .FirstOrDefaultAsync(x => x.Id == request.RoleId, cancellationToken: cancellationToken);
        if (role == null)
        {
            throw new RoleNotFoundException(request.RoleId);
        }

        var currentRole = await streamerDbContext.Roles.FirstOrDefaultAsync(
            x => x.StreamerId == currentUser.UserId && x.BroadcasterId == role.BroadcasterId,
            cancellationToken: cancellationToken
        );
        if (currentRole == null)
        {
            throw new ForbiddenException();
        }
        assignRole.EnsureCanAssign(currentRole, role.Type, request.Permissions);
        role.Permissions = request.Permissions;
        streamerDbContext.Roles.Update(role);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new EditRoleResponse(role.Id);
    }
}
