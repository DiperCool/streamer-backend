using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Streamers.Features.GetStreamerByUserName;

public record GetStreamerByUserName(string UserName) : IRequest<StreamerDto>;

public class GetStreamerByUserNameHandler(StreamerDbContext context)
    : IRequestHandler<GetStreamerByUserName, StreamerDto>
{
    public async Task<StreamerDto> Handle(
        GetStreamerByUserName request,
        CancellationToken cancellationToken
    )
    {
        Streamer? streamer = await context.Streamers.FirstOrDefaultAsync(
            x => x.UserName == request.UserName,
            cancellationToken: cancellationToken
        );
        if (streamer == null)
        {
            throw new NullReferenceException(
                $"Streamer with name {request.UserName} does not exist"
            );
        }

        return new StreamerDto
        {
            Id = streamer.Id,
            UserName = streamer.UserName,
            Avatar = streamer.Avatar,
            Followers = streamer.Followers,
        };
    }
}
