using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Vods.Models;

namespace Streamers.Features.Vods.Features.VodFinishProcess;

public record VodFinishProcessResponse(Guid VodId);

public record VodFinishProcess(Guid VodId, string Path, string Preview, long Duration)
    : IRequest<VodFinishProcessResponse>;

public class FinishProcessHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<VodFinishProcess, VodFinishProcessResponse>
{
    public async Task<VodFinishProcessResponse> Handle(
        VodFinishProcess request,
        CancellationToken cancellationToken
    )
    {
        Vod? vod = await streamerDbContext.Vods.FirstOrDefaultAsync(
            x => x.Id == request.VodId,
            cancellationToken: cancellationToken
        );
        if (vod == null)
        {
            return new VodFinishProcessResponse(Guid.Empty);
        }

        vod.Finish(request.Path, request.Preview, request.Duration, "title", "description");
        streamerDbContext.Vods.Update(vod);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new VodFinishProcessResponse(vod.Id);
    }
}
