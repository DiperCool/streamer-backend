using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Bots.Dtos;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SystemRoles.Services;

namespace Streamers.Features.Bots.Features.GetBots;

public record GetBots(string? Search, PagingArguments Paging, QueryContext<BotDto> QueryContext)
    : IRequest<Page<BotDto>>;

public class GetBotsHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser,
    ISystemRoleService systemRoleService
) : IRequestHandler<GetBots, Page<BotDto>>
{
    public async Task<Page<BotDto>> Handle(GetBots request, CancellationToken cancellationToken)
    {
        if (!await systemRoleService.HasAdministratorRole(currentUser.UserId))
        {
            throw new ForbiddenException();
        }
        var query = streamerDbContext.Bots.AsNoTracking();

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(x =>
                EF.Functions.ILike(x.Streamer.UserName!, $"%{request.Search}%")
            );
        }

        var dtoQuery = query.Select(x => new BotDto
        {
            Id = x.Id,
            StreamerId = x.StreamerId,
            State = x.State,
            StreamVideoUrl = x.StreamVideoUrl,
        });

        Page<BotDto> result = await dtoQuery
            .With(request.QueryContext)
            .ToPageAsync(request.Paging, cancellationToken: cancellationToken);

        return result;
    }
}
