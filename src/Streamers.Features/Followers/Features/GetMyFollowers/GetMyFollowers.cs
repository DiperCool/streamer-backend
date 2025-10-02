using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Followers.Dtos;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Followers.Features.GetMyFollowers;

public record GetMyFollowers(
    string? Search,
    QueryContext<FollowerDto> Query,
    PagingArguments PagingArguments
) : IRequest<Page<FollowerDto>>;

public class GetMyFollowersHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<GetMyFollowers, Page<FollowerDto>>
{
    public async Task<Page<FollowerDto>> Handle(
        GetMyFollowers request,
        CancellationToken cancellationToken
    )
    {
        var query = streamerDbContext
            .Followers.AsNoTracking()
            .Where(x => x.StreamerId == currentUser.UserId);
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(x =>
                EF.Functions.ILike(x.FollowerStreamer.UserName!, $"%{request.Search}%")
            );
        }

        var dtoQuery = query.Select(x => new FollowerDto
        {
            FollowerStreamerId = x.FollowerStreamerId,
            FollowedAt = x.FollowedAt,
        });

        Page<FollowerDto> result = await dtoQuery
            .With(request.Query)
            .ToPageAsync(request.PagingArguments, cancellationToken: cancellationToken);

        return result;
    }
}
