using GreenDonut.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Dtos;

namespace Streamers.Features.Streamers.Features.GetStreamers;

public record GetStreamers(
    string? Search,
    QueryContext<StreamerDto> Query,
    PagingArguments PagingArguments
) : IRequest<Page<StreamerDto>>;

public class GetStreamersHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<GetStreamers, Page<StreamerDto>>
{
    public async Task<Page<StreamerDto>> Handle(
        GetStreamers request,
        CancellationToken cancellationToken
    )
    {
        var query = streamerDbContext.Streamers.AsNoTracking();

        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(x => EF.Functions.ILike(x.UserName!, $"%{request.Search}%"));
        }

        var dtoQuery = query.Select(x => new StreamerDto
        {
            Id = x.Id,
            UserName = x.UserName,
            Avatar = x.Avatar,
            Followers = x.Followers,
            IsLive = x.IsLive,
        });

        Page<StreamerDto> result = await dtoQuery
            .With(request.Query)
            .ToPageAsync(request.PagingArguments, cancellationToken: cancellationToken);

        return result;
    }
}
