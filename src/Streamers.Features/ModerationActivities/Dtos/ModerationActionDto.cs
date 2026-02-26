using HotChocolate;
using HotChocolate.Types;
using Streamers.Features.Chats.Dtos;
using Streamers.Features.Chats.GraphQl; // Added for ChatMessagesDataLoader
using Streamers.Features.ModerationActivities.Graphql;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.GraphqQl;

namespace Streamers.Features.ModerationActivities.Dtos;

[UnionType("ModeratorAction")]
public abstract class ModeratorActionDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ModeratorId { get; init; }
    public required string StreamerId { get; init; }
    public required DateTimeOffset CreatedDate { get; init; }

    public Task<StreamerDto?> GetModerator(
        [Parent] ModeratorActionDto parent,
        StreamersByIdDataLoader streamerDataLoader,
        CancellationToken cancellationToken
    )
    {
        return streamerDataLoader.LoadAsync(parent.ModeratorId, cancellationToken);
    }
}

[ObjectType]
public class BanActionDto : ModeratorActionDto, IModeratorAction
{
    public required string TargetUserId { get; init; }
    public required DateTime? BannedUntil { get; init; }
    public required string? Reason { get; init; }

    public Task<StreamerDto?> GetTargetUser(
        [Parent] BanActionDto parent,
        StreamersByIdDataLoader streamerDataLoader,
        CancellationToken cancellationToken
    )
    {
        return streamerDataLoader.LoadAsync(parent.TargetUserId, cancellationToken);
    }
}

[ObjectType]
public class StreamNameActionDto : ModeratorActionDto, IModeratorAction
{
    public required string NewStreamName { get; init; }
}

[ObjectType]
public class StreamCategoryActionDto : ModeratorActionDto, IModeratorAction
{
    public required string NewCategory { get; init; }
}

[ObjectType]
public class StreamLanguageActionDto : ModeratorActionDto, IModeratorAction
{
    public required string NewLanguage { get; init; }
}

[ObjectType]
public class PinActionDto : ModeratorActionDto, IModeratorAction
{
    public required Guid ChatMessageId { get; init; }

    public Task<ChatMessageDto?> GetChatMessage(
        [Parent] PinActionDto parent,
        IChatMessagesByIdDataLoader chatMessagesDataLoader,
        CancellationToken cancellationToken
    )
    {
        return chatMessagesDataLoader.LoadAsync(parent.ChatMessageId, cancellationToken);
    }
}

[ObjectType]
public class UnpinActionDto : ModeratorActionDto, IModeratorAction
{
    public required Guid ChatMessageId { get; init; }

    public Task<ChatMessageDto?> GetChatMessage(
        [Parent] UnpinActionDto parent,
        IChatMessagesByIdDataLoader chatMessagesDataLoader,
        CancellationToken cancellationToken
    )
    {
        return chatMessagesDataLoader.LoadAsync(parent.ChatMessageId, cancellationToken);
    }
}

[ObjectType]
public class UnbanActionDto : ModeratorActionDto, IModeratorAction
{
    public required string TargetUserId { get; init; }

    public Task<StreamerDto?> GetTargetUser(
        [Parent] UnbanActionDto parent,
        StreamersByIdDataLoader streamerDataLoader,
        CancellationToken cancellationToken
    )
    {
        return streamerDataLoader.LoadAsync(parent.TargetUserId, cancellationToken);
    }
}

[ObjectType]
public class ChatModeActionDto : ModeratorActionDto, IModeratorAction
{
    public required string NewChatMode { get; init; }
}
