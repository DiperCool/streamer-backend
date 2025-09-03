using GreenDonut.Data;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Roles.Dtos;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Roles.Features.GetRoles;

public record GetRoles(
    RoleType RoleType,
    string BroadcasterId,
    PagingArguments Paging,
    QueryContext<RoleDto> QueryContext
) : IRequest<Page<RoleDto>>;

public class GetRolesHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetRoles, Page<RoleDto>>
{
    public async Task<Page<RoleDto>> Handle(GetRoles request, CancellationToken cancellationToken)
    {
        Page<RoleDto> result = await streamerDbContext
            .Roles.Where(x =>
                x.BroadcasterId == request.BroadcasterId && x.Type == request.RoleType
            )
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
