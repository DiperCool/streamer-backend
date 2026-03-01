using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Categories.Models;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Tags.Services;
using Streamers.Features.Vods.Enums;
using Streamers.Features.Vods.Exceptions;
using Streamers.Features.Vods.Models;

namespace Streamers.Features.Vods.Features.UpdateVod;

public record UpdateVodResponse(Guid Id);

public record UpdateVod(
    Guid Id,
    VodType Type,
    string Title,
    string Description,
    string Language,
    Guid? CategoryId,
    List<string> Tags
) : IRequest<UpdateVodResponse>;

public class UpdateVodHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser,
    ITagsService tagsService
) : IRequestHandler<UpdateVod, UpdateVodResponse>
{
    public async Task<UpdateVodResponse> Handle(
        UpdateVod request,
        CancellationToken cancellationToken
    )
    {
        var role = await streamerDbContext.Roles.FirstOrDefaultAsync(
            x => x.StreamerId == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        var query = streamerDbContext.Vods.AsNoTracking();
        if (role == null || !role.Permissions.HasPermission(Permissions.Roles))
        {
            query = query.Where(x => x.Type == VodType.Public);
        }

        Vod? vod = await query
            .Where(x => x.Id == request.Id)
            .Include(x => x.Tags)
            .Include(x => x.Category)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (vod == null)
        {
            throw new VodNotFoundException(request.Id);
        }

        Category? category = await streamerDbContext.Categories.FirstOrDefaultAsync(
            x => x.Id == request.CategoryId,
            cancellationToken: cancellationToken
        );
        var tags = await tagsService.Create(request.Tags);
        vod.Update(
            request.Type,
            request.Title,
            request.Description,
            category,
            tags,
            request.Language
        );
        streamerDbContext.Vods.Update(vod);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new UpdateVodResponse(vod.Id);
    }
}
