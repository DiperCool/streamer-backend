using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Dtos;

namespace Streamers.Features.Streamers.Features.GetStreamersByIds;

public record GetStreamersByIdsResponse(IDictionary<string, StreamerDto> Streamers);

public record GetStreamersByIds(IReadOnlyList<string> Ids) : IRequest<GetStreamersByIdsResponse>;

public class GetStreamersByIdsHandler(StreamerDbContext context)
    : IRequestHandler<GetStreamersByIds, GetStreamersByIdsResponse>
{
    public async Task<GetStreamersByIdsResponse> Handle(
        GetStreamersByIds request,
        CancellationToken cancellationToken
    )
    {
        var streamers = await context
            .Streamers.Where(s => request.Ids.Contains(s.Id))
            .Select(s => new StreamerDto
            {
                Id = s.Id,
                UserName = s.UserName,
                Avatar = s.Avatar,
                Followers = s.Followers,
                IsLive = s.IsLive,
            })
            .ToListAsync(cancellationToken);

        var dict = streamers.ToDictionary(s => s.Id, s => s);

        return new GetStreamersByIdsResponse(dict);
    }
}
