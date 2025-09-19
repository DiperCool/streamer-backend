using GreenDonut.Data;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Notifications.Dtos;
using Streamers.Features.Notifications.Features.GetNotifications;
using Streamers.Features.Notifications.Features.GetNotificationSettings;

namespace Streamers.Features.Notifications.Graphql;

[QueryType]
[Authorize]
public static partial class NotificationQuery
{
    [UsePaging(MaxPageSize = 10)]
    [UseFiltering]
    [UseSorting]
    public static async Task<Connection<NotificationDto>> GetNotifications(
        [Service] IMediator mediator,
        QueryContext<NotificationDto> queryContext,
        PagingArguments pagingArguments
    )
    {
        var res = await mediator.Send(new GetNotifications(pagingArguments, queryContext));
        return res.ToConnection();
    }

    public static async Task<NotificationSettingsDto> GetNotificationSettings(
        [Service] IMediator mediator
    )
    {
        return await mediator.Send(new GetNotificationSettings());
    }
}
