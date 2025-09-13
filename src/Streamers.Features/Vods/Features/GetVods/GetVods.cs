using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Vods.Dtos;
using Streamers.Features.Vods.Enums;

namespace Streamers.Features.Vods.Features.GetVods;

public record GetVods(string StreamerId, PagingArguments Paging, QueryContext<VodDto> QueryContext)
    : IRequest<Page<VodDto>>;

public class GetVodsHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<GetVods, Page<VodDto>>
{
    public async Task<Page<VodDto>> Handle(GetVods request, CancellationToken cancellationToken)
    {
        var role = currentUser.IsAuthenticated
            ? await streamerDbContext.Roles.FirstOrDefaultAsync(
                x => x.StreamerId == currentUser.UserId,
                cancellationToken: cancellationToken
            )
            : null;
        var query = streamerDbContext.Vods.AsNoTracking();
        if (role == null || !role.Permissions.HasPermission(Permissions.Roles))
        {
            query = query.Where(x => x.Type == VodType.Public);
        }
        Page<VodDto> result = await query
            .Where(x => x.StreamerId == request.StreamerId && x.Status == VodStatus.Finished)
            .Select(x => new VodDto
            {
                Id = x.Id,
                CreatedAt = x.CreatedAt,
                Source = x.Source,
                Title = x.Title,
                Preview = x.Preview,
                Description = x.Description,
                Views = x.Views,
                StreamerId = x.StreamerId,
                Duration = x.Duration,
                CategoryId = x.CategoryId,
                Language = x.Language,
                Type = x.Type,
            })
            .With(request.QueryContext)
            .ToPageAsync(request.Paging, cancellationToken: cancellationToken);
        return result;
    }
}
