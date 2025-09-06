using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Tags.Services;

namespace Streamers.Features.StreamInfos.Features.UpdateStreamInfo;

public record UpdateStreamInfoResponse(Guid Id);

public record UpdateStreamInfo(
    string StreamerId,
    string Language,
    string Title,
    Guid? CategoryId,
    List<string> Tags
) : IRequest<UpdateStreamInfoResponse>;

public class UpdateStreamInfoHandler(
    StreamerDbContext streamerDbContext,
    ICurrentUser currentUser,
    ITagsService tagsService
) : IRequestHandler<UpdateStreamInfo, UpdateStreamInfoResponse>
{
    public async Task<UpdateStreamInfoResponse> Handle(
        UpdateStreamInfo request,
        CancellationToken cancellationToken
    )
    {
        var streamer = await streamerDbContext.Roles.FirstOrDefaultAsync(
            x => x.StreamerId == currentUser.UserId,
            cancellationToken: cancellationToken
        );
        if (streamer == null)
        {
            throw new InvalidOperationException(
                $"Could not find streamer with id: {request.StreamerId}"
            );
        }

        if (!streamer.Permissions.HasPermission(Permissions.Stream))
        {
            throw new UnauthorizedAccessException();
        }
        var streamInfo = await streamerDbContext
            .StreamInfos.Include(x => x.Tags)
            .FirstOrDefaultAsync(
                x => x.StreamerId == request.StreamerId,
                cancellationToken: cancellationToken
            );
        if (streamInfo == null)
        {
            throw new InvalidOperationException("Could not find stream info");
        }
        var category = await streamerDbContext.Categories.FirstOrDefaultAsync(
            x => x.Id == request.CategoryId,
            cancellationToken: cancellationToken
        );
        var tags = await tagsService.Create(request.Tags);
        streamInfo.Update(request.Title, category, tags, request.Language);
        streamerDbContext.StreamInfos.Update(streamInfo);

        var currentStream = await streamerDbContext
            .Streams.Include(x => x.Tags)
            .FirstOrDefaultAsync(
                x => x.StreamerId == request.StreamerId && x.Active,
                cancellationToken: cancellationToken
            );
        if (currentStream != null)
        {
            currentStream.Update(request.Title, request.Language, tags, category);
            streamerDbContext.Streams.Update(currentStream);
        }

        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new UpdateStreamInfoResponse(streamInfo.Id);
    }
}
