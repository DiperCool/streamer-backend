using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SystemRoles.Dtos;
using Streamers.Features.SystemRoles.Exceptions;

namespace Streamers.Features.SystemRoles.Features.GetMySystemRole;

public record GetMySystemRole() : IRequest<SystemRoleDto>;

public class GetMySystemRoleHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<GetMySystemRole, SystemRoleDto>
{
    public async Task<SystemRoleDto> Handle(
        GetMySystemRole request,
        CancellationToken cancellationToken
    )
    {
        var role = await streamerDbContext.SystemRoles.FirstOrDefaultAsync(
            x => x.StreamerId == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (role == null)
        {
            throw new SystemRoleNotFoundException(currentUser.UserId);
        }

        return new SystemRoleDto { StreamerId = role.StreamerId, RoleType = role.RoleType };
    }
}
