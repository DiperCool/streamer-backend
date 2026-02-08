using GreenDonut.Data;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Roles.Features.GetMyRoles;

public record GetMyRoles(PagingArguments Paging, QueryContext<RoleDto> QueryContext)
    : IRequest<Page<RoleDto>>;

public class GetMyRolesHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<GetMyRoles, Page<RoleDto>>
{
    public async Task<Page<RoleDto>> Handle(GetMyRoles request, CancellationToken cancellationToken)
    {
        Page<RoleDto> result = await streamerDbContext
            .Roles.Where(x => x.StreamerId == currentUser.UserId)
            .Select(x => new RoleDto
            {
                Id = x.Id,
                StreamerId = x.StreamerId,
                Type = x.Type,
                BroadcasterId = x.BroadcasterId,
                Permissions = x.Permissions,
            })
            .With(request.QueryContext)
            .ToPageAsync(request.Paging, cancellationToken: cancellationToken);
        return result;
    }
}
