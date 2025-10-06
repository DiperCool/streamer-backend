using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Bots.Dtos;
using Streamers.Features.Bots.Enums;
using Streamers.Features.Bots.Models;
using Streamers.Features.Shared.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.SystemRoles.Services;

namespace Streamers.Features.Bots.Features.EditBot;

public record EditBotResponse(Guid Id);

[Transactional]
public record EditBot(Guid Id, BotState State, string StreamVideoUrl) : IRequest<EditBotResponse>;

public class EditBotHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser,
    ISystemRoleService systemRoleService,
    ICapPublisher capPublisher
) : IRequestHandler<EditBot, EditBotResponse>
{
    public async Task<EditBotResponse> Handle(EditBot request, CancellationToken cancellationToken)
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
        bot.Edit(request.State, request.StreamVideoUrl);
        streamerDbContext.Bots.Update(bot);
        await capPublisher.PublishAsync(
            nameof(EditBot),
            BotEventDto.Create(bot),
            cancellationToken: cancellationToken
        );
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new EditBotResponse(bot.Id);
    }
}
