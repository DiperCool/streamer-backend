using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.StreamInfos.Dtos;

namespace Streamers.Features.StreamInfos.Features.GetStreamInfo;

public record GetStreamInfo(string StreamerId) : IRequest<StreamInfoDto>;

public partial class GetStreamInfoHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetStreamInfo, StreamInfoDto>
{
    public async Task<StreamInfoDto> Handle(
        GetStreamInfo request,
        CancellationToken cancellationToken
    )
    {
        var info = await streamerDbContext.StreamInfos.FirstOrDefaultAsync(x =>
            x.StreamerId == request.StreamerId
        );
        if (info == null)
        {
            throw new InvalidOperationException(
                $"Could not find stream info with id {request.StreamerId}"
            );
        }

        return new StreamInfoDto
        {
            StreamerId = info.StreamerId,
            CategoryId = info.CategoryId,
            Title = info.Title,
            Id = info.Id,
            Language = info.Language,
        };
    }
}
