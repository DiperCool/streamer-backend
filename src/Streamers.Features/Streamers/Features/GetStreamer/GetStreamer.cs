using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streamers.Dtos;
using Streamers.Features.Streamers.Models;

namespace Streamers.Features.Streamers.Features.GetStreamer;

public record GetStreamer(string StreamerId) : IRequest<StreamerDto>;

public class GetStreamerHandler(StreamerDbContext context)
    : IRequestHandler<GetStreamer, StreamerDto>
{
    public async Task<StreamerDto> Handle(GetStreamer request, CancellationToken cancellationToken)
    {
        Streamer? streamer = await context.Streamers.FirstOrDefaultAsync(
            x => x.Id == request.StreamerId,
            cancellationToken: cancellationToken
        );
        if (streamer == null)
        {
            throw new NullReferenceException(
                $"Streamer with id {request.StreamerId} does not exist"
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
