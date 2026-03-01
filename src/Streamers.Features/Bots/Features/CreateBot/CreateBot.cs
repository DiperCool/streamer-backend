using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Bots.Dtos;
using Streamers.Features.Bots.Enums;
using Streamers.Features.Bots.Models;
using Streamers.Features.Shared.Cqrs;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Exceptions;
using Streamers.Features.SystemRoles.Services;

namespace Streamers.Features.Bots.Features.CreateBot;

public record CreateBotResponse(Guid Id);

[Transactional]
public record CreateBot(string StreamerId, BotState State, string StreamVideoUrl)
    : IRequest<CreateBotResponse>;

public class CreateBotHandler(
    StreamerDbContext streamerDbContext,
    ISystemRoleService systemRoleService,
    ICurrentUser currentUser,
    ICapPublisher capPublisher
) : IRequestHandler<CreateBot, CreateBotResponse>
{
    public async Task<CreateBotResponse> Handle(
        CreateBot request,
        CancellationToken cancellationToken
    )
    {
        if (!await systemRoleService.HasAdministratorRole(currentUser.UserId))
        {
            throw new ForbiddenException();
        }
        var streamer = await streamerDbContext
            .Streamers.Include(x => x.StreamSettings)
            .FirstOrDefaultAsync(
                x => x.Id == request.StreamerId,
                cancellationToken: cancellationToken
            );
        if (streamer == null)
        {
            throw new StreamerNotFoundException(request.StreamerId);
        }

        Bot bot = new Bot(streamer, request.State, request.StreamVideoUrl);
        await streamerDbContext.Bots.AddAsync(bot, cancellationToken);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        await capPublisher.PublishAsync(
            nameof(CreateBot),
            BotEventDto.Create(bot),
            cancellationToken: cancellationToken
        );
        return new CreateBotResponse(bot.Id);
    }
}
