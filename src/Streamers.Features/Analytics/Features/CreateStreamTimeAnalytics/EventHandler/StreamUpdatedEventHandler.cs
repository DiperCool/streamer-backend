using Shared.Abstractions.Domain;
using Streamers.Features.Analytics.Enums;
using Streamers.Features.Analytics.Models;
using Streamers.Features.Shared.Persistance;
using Streamers.Features.Streams.Models;
using Stream = Streamers.Features.Streams.Models.Stream;

namespace Streamers.Features.Analytics.Features.CreateStreamTimeAnalytics.EventHandler;

public class StreamUpdatedEventHandler(StreamerDbContext streamerDbContext)
    : IDomainEventHandler<StreamUpdated>
{
    public async Task Handle(StreamUpdated domainEvent, CancellationToken cancellationToken)
    {
        Stream stream = domainEvent.Stream;
        if (stream.Active)
        {
            return;
        }

        AnalyticsItem analyticsItem = new AnalyticsItem(
            stream.Duration ?? 0,
            DateTime.UtcNow,
            AnalyticsItemType.StreamTime,
            stream.StreamerId
        );
        await streamerDbContext.AnalyticsItems.AddAsync(analyticsItem, cancellationToken);
        await streamerDbContext.SaveChangesAsync(cancellationToken);
    }
}
