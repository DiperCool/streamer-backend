using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.Models;

namespace Streamers.Features.Chats.GraphQl;

[SubscriptionType]
public static partial class BannedUserSubscription
{
    [Subscribe]
    [Topic($"{nameof(UserBanned)}-{{{nameof(broadcasterId)}}}-{{{nameof(userId)}}}")]
    public static BannedUserDto UserBanned(
        string userId,
        string broadcasterId,
        [EventMessage] BannedUserDto message
    ) => message;

    [Subscribe]
    [Topic($"{nameof(UserUnbanned)}-{{{nameof(broadcasterId)}}}-{{{nameof(userId)}}}")]
    public static BannedUserDto UserUnbanned(
        string userId,
        string broadcasterId,
        [EventMessage] BannedUserDto message
    ) => message;
}
