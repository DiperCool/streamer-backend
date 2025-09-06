using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Vods.Dtos;

namespace Streamers.Features.Vods.Features.GetVod;

public record GetVod(Guid VodId) : IRequest<VodDto>;

public class GetVodHandler(StreamerDbContext streamerDbContext) : IRequestHandler<GetVod, VodDto>
{
    public async Task<VodDto> Handle(GetVod request, CancellationToken cancellationToken)
    {
        VodDto? vod = await streamerDbContext
            .Vods.AsNoTracking()
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
