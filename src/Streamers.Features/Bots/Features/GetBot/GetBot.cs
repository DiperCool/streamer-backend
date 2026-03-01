using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Bots.Dtos;
using Streamers.Features.Bots.Exceptions;
using Streamers.Features.Bots.Models;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SystemRoles.Services;

namespace Streamers.Features.Bots.Features.GetBot;

public record GetBot(Guid Id) : IRequest<BotDto>;

public class GetBotHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser,
    ISystemRoleService systemRoleService
) : IRequestHandler<GetBot, BotDto>
{
    public async Task<BotDto> Handle(GetBot request, CancellationToken cancellationToken)
    {
        if (!await systemRoleService.HasAdministratorRole(currentUser.UserId))
        {
            throw new ForbiddenException();
        }
        Bot? bot = await streamerDbContext
            .Bots.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (bot == null)
        {
            throw new BotNotFoundException(request.Id);
        }

        return new BotDto
        {
            Id = bot.Id,
            StreamerId = bot.StreamerId,
            State = bot.State,
            StreamVideoUrl = bot.StreamVideoUrl,
        };
    }
}
