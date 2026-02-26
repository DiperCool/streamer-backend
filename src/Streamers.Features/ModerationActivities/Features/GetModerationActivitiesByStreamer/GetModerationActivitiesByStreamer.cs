using System.Collections.Immutable;
using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.ModerationActivities.Dtos;
using Streamers.Features.ModerationActivities.Models;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.ModerationActivities.Features.GetModerationActivitiesByStreamer;

public record GetModerationActivitiesByStreamer(string StreamerId, PagingArguments PagingArguments)
    : IRequest<Page<ModeratorActionDto>>;

public class GetModerationActivitiesByStreamerHandler(
    StreamerDbContext dbContext,
    IRoleService roleService,
    ICurrentUser currentUser
) : IRequestHandler<GetModerationActivitiesByStreamer, Page<ModeratorActionDto>>
{
    public async Task<Page<ModeratorActionDto>> Handle(
        GetModerationActivitiesByStreamer request,
        CancellationToken cancellationToken
    )
    {
        if (!await roleService.HasRole(request.StreamerId, currentUser.UserId, Permissions.Chat))
        {
            throw new UnauthorizedAccessException(
                "You do not have permission to view moderation activities for this streamer."
            );
        }

        var query = dbContext
            .ModeratorActionTypes.AsNoTracking()
            .Where(x => x.StreamerId == request.StreamerId)
            .OrderByDescending(x => x.CreatedDate);

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
                            StreamerId = banAction.StreamerId,
                            CreatedDate = banAction.CreatedDate,
                            TargetUserId = banAction.TargetUserId,
                            BannedUntil = banAction.BannedUntil,
                            Reason = banAction.Reason,
                        },
                    UnbanAction unbanAction => new UnbanActionDto
                    {
                        Id = unbanAction.Id,
                        Name = unbanAction.Name,
                        ModeratorId = unbanAction.ModeratorId,
                        StreamerId = unbanAction.StreamerId,
                        CreatedDate = unbanAction.CreatedDate,
                        TargetUserId = unbanAction.TargetUserId,
                    },
                    Models.PinAction pinAction => new PinActionDto
                    {
                        Id = pinAction.Id,
                        Name = pinAction.Name,
                        ModeratorId = pinAction.ModeratorId,
                        StreamerId = pinAction.StreamerId,
                        CreatedDate = pinAction.CreatedDate,
                        ChatMessageId = pinAction.ChatMessageId,
                    },
                    Models.UnpinAction unpinAction => new UnpinActionDto
                    {
                        Id = unpinAction.Id,
                        Name = unpinAction.Name,
                        ModeratorId = unpinAction.ModeratorId,
                        StreamerId = unpinAction.StreamerId,
                        CreatedDate = unpinAction.CreatedDate,
                        ChatMessageId = unpinAction.ChatMessageId,
                    },
                    StreamCategoryAction categoryAction => new StreamCategoryActionDto
                    {
                        Id = categoryAction.Id,
                        Name = categoryAction.Name,
                        ModeratorId = categoryAction.ModeratorId,
                        StreamerId = categoryAction.StreamerId,
                        CreatedDate = categoryAction.CreatedDate,
                        NewCategory = categoryAction.NewCategory,
                    },
                    StreamLanguageAction languageAction => new StreamLanguageActionDto
                    {
                        Id = languageAction.Id,
                        Name = languageAction.Name,
                        ModeratorId = languageAction.ModeratorId,
                        StreamerId = languageAction.StreamerId,
                        CreatedDate = languageAction.CreatedDate,
                        NewLanguage = languageAction.NewLanguage,
                    },
                    StreamNameAction nameAction => new StreamNameActionDto
                    {
                        Id = nameAction.Id,
                        Name = nameAction.Name,
                        ModeratorId = nameAction.ModeratorId,
                        StreamerId = nameAction.StreamerId,
                        CreatedDate = nameAction.CreatedDate,
                        NewStreamName = nameAction.NewStreamName,
                    },
                    ChatModeAction modeAction => new ChatModeActionDto
                    {
                        Id = modeAction.Id,
                        Name = modeAction.Name,
                        ModeratorId = modeAction.ModeratorId,
                        StreamerId = modeAction.StreamerId,
                        CreatedDate = modeAction.CreatedDate,
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
