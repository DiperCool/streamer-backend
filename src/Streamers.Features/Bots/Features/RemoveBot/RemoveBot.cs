using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Bots.Dtos;
using Streamers.Features.Bots.Models;
using Streamers.Features.Shared.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SystemRoles.Services;

namespace Streamers.Features.Bots.Features.RemoveBot;

public record RemoveBotResponse(Guid Id);

[Transactional]
public record RemoveBot(Guid Id) : IRequest<RemoveBotResponse>;

public class RemoveBotHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser,
    ISystemRoleService systemRoleService,
    ICapPublisher capPublisher
) : IRequestHandler<RemoveBot, RemoveBotResponse>
{
    public async Task<RemoveBotResponse> Handle(
        RemoveBot request,
        CancellationToken cancellationToken
    )
    {
        if (!await systemRoleService.HasAdministratorRole(currentUser.UserId))
        {
            throw new UnauthorizedAccessException();
        }
        Bot? bot = await streamerDbContext
            .Bots.Include(x => x.Streamer)
            .ThenInclude(x => x.StreamSettings)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (bot == null)
        {
            throw new InvalidOperationException("Bot not found");
        }
        await capPublisher.PublishAsync(
            nameof(RemoveBot),
            BotEventDto.Create(bot),
            cancellationToken: cancellationToken
        );
        streamerDbContext.Bots.Remove(bot);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new RemoveBotResponse(bot.Id);
    }
}
