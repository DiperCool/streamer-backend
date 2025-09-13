using Hangfire;
using HotChocolate.Subscriptions;
using Shared.Abstractions.Cqrs;
using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Chats.Features.BanUser;

public class UserBannedEventHandler(ITopicEventSender sender) : IDomainEventHandler<UserBanned>
{
    public async Task Handle(UserBanned domainEvent, CancellationToken cancellationToken)
    {
        var message = domainEvent.BannedUser;
        var dto = new BannedUserDto
        {
            Id = message.Id,
            UserId = message.UserId,
            BannedById = message.BannedById,
            BannedAt = message.BannedAt,
            BannedUntil = message.BannedUntil,
            Reason = message.Reason,
        };

        await sender.SendAsync(
            $"{nameof(UserBanned)}-{message.BroadcasterId}-{message.UserId}",
            dto,
            cancellationToken
        );
    }
}
