using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Roles.Enums;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Vods.Dtos;
using Streamers.Features.Vods.Enums;

namespace Streamers.Features.Vods.Features.GetVod;

public record GetVod(Guid VodId) : IRequest<VodDto>;

public class GetVodHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<GetVod, VodDto>
{
    public async Task<VodDto> Handle(GetVod request, CancellationToken cancellationToken)
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
        VodDto? vod = await query
            .Where(x => x.Id == request.VodId)
            .Select(x => new VodDto
            {
                Id = x.Id,
                Source = x.Source,
                Title = x.Title,
                Preview = x.Preview,
                Description = x.Description,
                Views = x.Views,
                CreatedAt = x.CreatedAt,
                StreamerId = x.StreamerId,
                Duration = x.Duration,
                CategoryId = x.CategoryId,
                Language = x.Language,
                Type = x.Type,
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (vod == null)
        {
            throw new InvalidOperationException("Vod not found");
        }
        await streamerDbContext
            .Vods.Where(s => s.Id == request.VodId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(s => s.Views, s => s.Views + 1),
                cancellationToken
            );
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return vod;
    }
}
