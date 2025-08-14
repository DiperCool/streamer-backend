using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;

namespace Streamers.Features.Streams.Features.CanStartStream;

public record CanStartStreamResponse(bool Response);

public record CanStartStream(string Token, string StreamName, string Action, string Schema)
    : IRequest<CanStartStreamResponse>;

public class CanStartStreamHandler(StreamerDbContext dbContext)
    : IRequestHandler<CanStartStream, CanStartStreamResponse>
{
    public async Task<CanStartStreamResponse> Handle(
        CanStartStream request,
        CancellationToken cancellationToken
    )
    {
        const string action = "publish";
        const string schema = "rtmp";
        if (action != request.Action || schema != request.Schema)
        {
            return new CanStartStreamResponse(false);
        }

        return new CanStartStreamResponse(
            await dbContext.StreamSettings.AnyAsync(
                x => x.StreamName == request.StreamName && x.StreamKeyToken == request.Token,
                cancellationToken: cancellationToken
            )
        );
    }
}
