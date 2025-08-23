using System.Runtime.CompilerServices;
using Hangfire;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using Shared.Extensions;
using Streamers.Features.AntMedia.Services;

namespace Streamers.Features.Streams.GraphQl;

public record StreamWatcher(Guid StreamId);

[SubscriptionType]
public static partial class WatchStreamSubscription
{
    public static async IAsyncEnumerable<StreamWatcher> SubscribeWatchStream(
        [Service] ITopicEventReceiver receiver,
        IHttpContextAccessor contextAccessor,
        Guid streamId,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        string topic = $"{nameof(WatchStream)}-{streamId}";
        string? ipAddress = contextAccessor.HttpContext?.GetClientIpAddress();
        if (ipAddress == null)
        {
            yield break;
        }
        BackgroundJob.Enqueue<AddReaderHandler>(x => x.HandleAsync(streamId));

        ISourceStream<StreamWatcher> stream = await receiver.SubscribeAsync<StreamWatcher>(
            topic,
            cancellationToken
        );
        try
        {
            await foreach (
                StreamWatcher evt in stream.ReadEventsAsync().WithCancellation(cancellationToken)
            )
            {
                yield return evt;
            }
        }
        finally
        {
            BackgroundJob.Enqueue<RemoveReaderHandler>(x => x.HandleAsync(streamId));
        }
    }

    [Subscribe(With = nameof(SubscribeWatchStream))]
    public static StreamWatcher WatchStream(
        [EventMessage] StreamWatcher streamWatcher,
        Guid streamId
    ) => streamWatcher;
}
