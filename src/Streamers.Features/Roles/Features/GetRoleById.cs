using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Roles.Dtos;
using Streamers.Features.Roles.Exceptions;
using Streamers.Features.Roles.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Roles.Features;

public record GetRoleById(Guid RoleId) : IRequest<RoleDto>;

public class GetRoleByIdHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetRoleById, RoleDto>
{
    public async Task<RoleDto> Handle(GetRoleById request, CancellationToken cancellationToken)
    {
        Role? role = await streamerDbContext.Roles.FirstOrDefaultAsync(
            x => x.Id == request.RoleId,
            cancellationToken: cancellationToken
        );
        if (role == null)
        {
            throw new RoleNotFoundException(request.RoleId);
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
