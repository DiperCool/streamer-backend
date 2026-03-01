using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Chats.Exceptions;
using Streamers.Features.Chats.Models;
using Streamers.Features.ModerationActivities.Models;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Exceptions;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.UpdateChatSettings;

public record UpdateChatSettingsResponse(Guid Id);

public record UpdateChatSettings(
    Guid Id,
    int? SlowMode,
    bool FollowersOnly,
    bool SubscribersOnly,
    List<string> BannedWords
) : IRequest<UpdateChatSettingsResponse>;

public class UpdateChatSettingsHandler(
    StreamerDbContext streamerDbContext,
    IRoleService roleService,
    ICurrentUser currentUser
) : IRequestHandler<UpdateChatSettings, UpdateChatSettingsResponse>
{
    public async Task<UpdateChatSettingsResponse> Handle(
        UpdateChatSettings request,
        CancellationToken cancellationToken
    )
    {
        ChatSettings? chatSettings = await streamerDbContext
            .ChatSettings.Include(x => x.Chat)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (chatSettings == null)
        {
            throw new ChatSettingsNotFoundException(request.Id);
        }
        if (
            !await roleService.HasRole(
                chatSettings.StreamerId,
                currentUser.UserId,
                Permissions.Chat
            )
        )
        {
            throw new ForbiddenException();
        }

        if (chatSettings.SlowMode != request.SlowMode)
        {
            var newChatMode = request.SlowMode.HasValue ? $"Slow mode enabled: {request.SlowMode}s" : "Slow mode disabled";
            var action = new ChatModeAction(currentUser.UserId, chatSettings.StreamerId, newChatMode);
            await streamerDbContext.ModeratorActionTypes.AddAsync(action, cancellationToken);
        }

        if (chatSettings.FollowersOnly != request.FollowersOnly)
        {
            var newChatMode = request.FollowersOnly ? "Followers only enabled" : "Followers only disabled";
            var action = new ChatModeAction(currentUser.UserId, chatSettings.StreamerId, newChatMode);
            await streamerDbContext.ModeratorActionTypes.AddAsync(action, cancellationToken);
        }

        if (chatSettings.SubscribersOnly != request.SubscribersOnly)
        {
            var newChatMode = request.SubscribersOnly ? "Subscribers only enabled" : "Subscribers only disabled";
            var action = new ChatModeAction(currentUser.UserId, chatSettings.StreamerId, newChatMode);
            await streamerDbContext.ModeratorActionTypes.AddAsync(action, cancellationToken);
        }

        chatSettings.Update(
            request.SlowMode,
            request.FollowersOnly,
            request.SubscribersOnly,
            request.BannedWords
        );
        streamerDbContext.ChatSettings.Update(chatSettings);
        await streamerDbContext.SaveChangesAsync(cancellationToken: cancellationToken);
        return new UpdateChatSettingsResponse(chatSettings.Id);
    }
}
