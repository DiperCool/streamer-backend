using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Models;
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

public class UpdateChatSettingsHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<UpdateChatSettings, UpdateChatSettingsResponse>
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
            throw new InvalidOperationException("Chat settings not found");
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
