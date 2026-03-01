using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Dtos;
using Streamers.Features.Roles.Exceptions;
using Streamers.Features.Roles.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Roles.Features.GetMyRole;

public record GetMyRole(string BroadcasterId) : IRequest<RoleDto>;

public class GetMyRoleHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<GetMyRole, RoleDto>
{
    public async Task<RoleDto> Handle(GetMyRole request, CancellationToken cancellationToken)
    {
        Role? role = await streamerDbContext.Roles.FirstOrDefaultAsync(x =>
            x.BroadcasterId == request.BroadcasterId && x.StreamerId == currentUser.UserId
        );
        if (role == null)
        {
            throw new RoleNotFoundException(Guid.Empty);
        }

        return new RoleDto
        {
            Id = role.Id,
            StreamerId = role.StreamerId,
            Type = role.Type,
            BroadcasterId = role.BroadcasterId,
            Permissions = role.Permissions,
        };
    }
}
