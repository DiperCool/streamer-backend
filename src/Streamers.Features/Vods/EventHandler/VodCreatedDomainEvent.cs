using DotNetCore.CAP;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Abstractions.Domain;
using streamer.ServiceDefaults;
using Streamers.Features.Streamers.Services;
using Streamers.Features.Streams.Enums;
using Streamers.Features.Vods.Models;

namespace Streamers.Features.Vods.EventHandler;

public record VodProcess(Guid VodId, string HlsSource);

public class VodCreatedDomainEvent(ICapPublisher capPublisher) : IDomainEventHandler<VodCreated>
{
    public async Task Handle(VodCreated domainEvent, CancellationToken cancellationToken)
    {
        await capPublisher.PublishAsync(
            "process-vod",
            new VodProcess(domainEvent.Vod.Id, domainEvent.Vod.StreamHls),
            cancellationToken: cancellationToken
        );
    }
}
