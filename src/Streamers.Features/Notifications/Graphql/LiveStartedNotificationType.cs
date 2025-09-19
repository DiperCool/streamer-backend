// using HotChocolate;
// using HotChocolate.Types;
// using Streamers.Features.Notifications.Dtos;
// using Streamers.Features.Streamers.Dtos;
// using Streamers.Features.Streamers.GraphqQl;
//
// namespace Streamers.Features.Notifications.Graphql;
//
// [ObjectType<LiveStartedNotificationDto>]
// public static partial class LiveStartedNotificationType
// {
//     static partial void Configure(IObjectTypeDescriptor<LiveStartedNotificationDto> descriptor)
//     {
//         descriptor.Implements<NotificationInterface>();
//     }
//
//     public static async Task<StreamerDto?> GetStreamerAsync(
//         [Parent] LiveStartedNotificationDto notification,
//         IStreamersByIdDataLoader dataLoader,
//         CancellationToken cancellationToken
//     )
//     {
//         return await dataLoader.LoadAsync(notification.StreamerId, cancellationToken);
//     }
// }
