using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Shared.Persistance;
using Stream = Streamers.Features.Streams.Models.Stream;

namespace Streamers.Features.Streamers.Features.UpdateStreamPreview;

public class UpdateStreamPreviewResponse();

public record UpdateStreamPreview(Guid StreamId, string PreviewUrl)
    : IRequest<UpdateStreamPreviewResponse>;

public class UpdateStreamPreviewHandler(StreamerDbContext streamerDbContext)
    : IRequestHandler<UpdateStreamPreview, UpdateStreamPreviewResponse>
{
    public async Task<UpdateStreamPreviewResponse> Handle(
        UpdateStreamPreview request,
        CancellationToken cancellationToken
    )
    {
        Stream? stream = await streamerDbContext.Streams.FirstOrDefaultAsync(
            x => x.Id == request.StreamId,
            cancellationToken: cancellationToken
        );
        if (stream == null)
        {
            return new UpdateStreamPreviewResponse();
        }

        stream.Preview = request.PreviewUrl;
        streamerDbContext.Streams.Update(stream);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
        return new UpdateStreamPreviewResponse();
    }
}
