using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Followers.Dtos;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Dtos;

namespace Streamers.Features.Followers.Features.GetMyFollowings;

public record GetMyFollowings(
    string? Search,
    QueryContext<StreamerFollowerDto> Query,
    PagingArguments PagingArguments
) : IRequest<Page<StreamerFollowerDto>>;

public class GetMyFollowingsHandler(StreamerDbContext streamerDbContext, ICurrentUser currentUser)
    : IRequestHandler<GetMyFollowings, Page<StreamerFollowerDto>>
{
    public async Task<Page<StreamerFollowerDto>> Handle(
        GetMyFollowings request,
        CancellationToken cancellationToken
    )
    {
        var query = streamerDbContext
            .Followers.AsNoTracking()
            .Where(x => x.FollowerStreamerId == currentUser.UserId)
            .Select(x => x.Streamer);
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(x => EF.Functions.ILike(x.UserName!, $"%{request.Search}%"));
        }

        var dtoQuery = query.Select(x => new StreamerFollowerDto
        {
            Id = x.Id,
            UserName = x.UserName,
            Avatar = x.Avatar,
            IsLive = x.IsLive,
            CurrentStreamId = x.CurrentStreamId,
        });

        Page<StreamerFollowerDto> result = await dtoQuery
            .With(request.Query)
            .ToPageAsync(request.PagingArguments, cancellationToken: cancellationToken);

        return result;
    }
}
