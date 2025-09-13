using Hangfire;
using HotChocolate.Subscriptions;
using Shared.Abstractions.Domain;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.Models;

namespace Streamers.Features.Chats.Features.ClearBan;

public class UserUnbannedEventHandler(ITopicEventSender sender) : IDomainEventHandler<UserUnbanned>
{
    public async Task Handle(UserUnbanned domainEvent, CancellationToken cancellationToken)
    {
        var message = domainEvent.BannedUser;
        BackgroundJob.Delete(message.JobId);
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
            $"{nameof(UserUnbanned)}-{message.BroadcasterId}-{message.UserId}",
            dto,
            cancellationToken
        );
    }
}
