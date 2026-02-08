using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.GetChat;

public record GetChat(string StreamerId) : IRequest<ChatDto>;

public class GetChatHandler(StreamerDbContext streamerDbContext) : IRequestHandler<GetChat, ChatDto>
{
    public async Task<ChatDto> Handle(GetChat request, CancellationToken cancellationToken)
    {
        ChatDto? chat = await streamerDbContext
            .Chats.Select(x => new ChatDto
            {
                SettingsId = x.SettingsId,
                PinnedMessageId = x.PinnedMessageId,
                StreamerId = x.StreamerId,
                Id = x.Id,
            })
            .FirstOrDefaultAsync(
                x => x.StreamerId == request.StreamerId,
                cancellationToken: cancellationToken
            );
        if (chat == null)
        {
            throw new InvalidOperationException("Chat not found");
        }

        return chat;
    }
}
