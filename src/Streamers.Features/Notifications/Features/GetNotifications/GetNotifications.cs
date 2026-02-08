using GreenDonut.Data;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Notifications.Dtos;
using Streamers.Features.Notifications.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Notifications.Features.GetNotifications;

public record GetNotifications(PagingArguments Paging, QueryContext<NotificationDto> QueryContext)
    : IRequest<Page<NotificationDto>>;

public class GetNotificationsHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<GetNotifications, Page<NotificationDto>>
{
    public async Task<Page<NotificationDto>> Handle(
        GetNotifications request,
        CancellationToken cancellationToken
    )
    {
        var result = await streamerDbContext
            .Notifications.Where(x => x.UserId == currentUser.UserId)
            .Select(x => new NotificationDto
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Seen = x.Seen,
                Discriminator = x.Discriminator,
                StreamerId =
                    x.Discriminator == nameof(LiveStartedNotification)
                        ? ((LiveStartedNotification)x).StreamerId
                        : ((UserFollowedNotification)x).StreamerId,
            })
            .With(request.QueryContext)
            .ToPageAsync(request.Paging, cancellationToken: cancellationToken);

        return result;
    }
}
