using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Roles.Features.RemoveRole;

public record RemoveRoleResponse(Guid Id);

public record RemoveRole(Guid RoleId) : IRequest<RemoveRoleResponse>;

public class RemoveRoleHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<RemoveRole, RemoveRoleResponse>
{
    public async Task<RemoveRoleResponse> Handle(
        RemoveRole request,
        CancellationToken cancellationToken
    )
    {
        Role? role = await streamerDbContext.Roles.FirstOrDefaultAsync(
            r => r.Id == request.RoleId && r.BroadcasterId == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (role == null)
        {
            throw new InvalidOperationException("Could not find role");
        }
        streamerDbContext.Roles.Remove(role);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new RemoveRoleResponse(role.Id);
    }
}
