using System.Collections.Immutable;
using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.ModerationActivities.Dtos;
using Streamers.Features.ModerationActivities.Models;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.ModerationActivities.Features.GetModerationActivitiesByStreamer;

public record GetModerationActivitiesByStreamer(string StreamerId, PagingArguments PagingArguments)
    : IRequest<Page<ModeratorActionDto>>;

public class GetModerationActivitiesByStreamerHandler(StreamerDbContext dbContext)
    : IRequestHandler<GetModerationActivitiesByStreamer, Page<ModeratorActionDto>>
{
    public async Task<Page<ModeratorActionDto>> Handle(
        GetModerationActivitiesByStreamer request,
        CancellationToken cancellationToken
    )
    {
        var query = dbContext
            .ModeratorActionTypes.AsNoTracking()
            .Where(x => x.StreamerId == request.StreamerId)
            .OrderByDescending(x => x.Id);

        var pageOfEntities = await query.ToPageAsync(
            request.PagingArguments,
            cancellationToken: cancellationToken
        );

        var dtos = pageOfEntities
            .Items.Select(action =>
                action switch
                {
                    BanAction banAction => (ModeratorActionDto)
                        new BanActionDto
                        {
                            Id = banAction.Id,
                            Name = banAction.Name,
                            ModeratorId = banAction.ModeratorId,
                            TargetUserId = banAction.TargetUserId,
                            BannedUntil = banAction.BannedUntil,
                            Reason = banAction.Reason,
                        },
                    UnbanAction unbanAction => new UnbanActionDto
                    {
                        Id = unbanAction.Id,
                        Name = unbanAction.Name,
                        ModeratorId = unbanAction.ModeratorId,
                        TargetUserId = unbanAction.TargetUserId,
                    },
                    Models.PinAction pinAction => new PinActionDto
                    {
                        Id = pinAction.Id,
                        Name = pinAction.Name,
                        ModeratorId = pinAction.ModeratorId,
                        ChatMessageId = pinAction.ChatMessageId,
                    },
                    Models.UnpinAction unpinAction => new UnpinActionDto
                    {
                        Id = unpinAction.Id,
                        Name = unpinAction.Name,
                        ModeratorId = unpinAction.ModeratorId,
                        ChatMessageId = unpinAction.ChatMessageId,
                    },
                    StreamCategoryAction categoryAction => new StreamCategoryActionDto
                    {
                        Id = categoryAction.Id,
                        Name = categoryAction.Name,
                        ModeratorId = categoryAction.ModeratorId,
                        NewCategory = categoryAction.NewCategory,
                    },
                    StreamLanguageAction languageAction => new StreamLanguageActionDto
                    {
                        Id = languageAction.Id,
                        Name = languageAction.Name,
                        ModeratorId = languageAction.ModeratorId,
                        NewLanguage = languageAction.NewLanguage,
                    },
                    StreamNameAction nameAction => new StreamNameActionDto
                    {
                        Id = nameAction.Id,
                        Name = nameAction.Name,
                        ModeratorId = nameAction.ModeratorId,
                        NewStreamName = nameAction.NewStreamName,
                    },
                    ChatModeAction modeAction => new ChatModeActionDto
                    {
                        Id = modeAction.Id,
                        Name = modeAction.Name,
                        ModeratorId = modeAction.ModeratorId,
                        NewChatMode = modeAction.NewChatMode,
                    },
                    _ => null,
                }
            )
            .ToImmutableArray();

        var entityMap = pageOfEntities.Items.ToDictionary(e => e.Id);

        var result = new Page<ModeratorActionDto>(
            dtos!,
            pageOfEntities.HasNextPage,
            pageOfEntities.HasPreviousPage,
            dto => pageOfEntities.CreateCursor(entityMap[dto.Id]),
            pageOfEntities.TotalCount
        );

        return result;
    }
}
