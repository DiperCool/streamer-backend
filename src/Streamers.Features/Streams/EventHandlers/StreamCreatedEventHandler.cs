// using DotNetCore.CAP;
// using Microsoft.Extensions.Logging;
// using Shared.Abstractions.Domain;
// using Streamers.Features.Shared.Persistance;
// using Streamers.Features.Streams.Enums;
// using Streamers.Features.Streams.Models;
// using Streamers.Features.Vods.Models;
//
// namespace Streamers.Features.Streams.EventHandlers;
//
// public class StreamCreatedEventHandler(
//     StreamerDbContext streamerDbContext,
//     ILogger<StreamCreatedEventHandler> logger
// ) : IDomainEventHandler<StreamCreated>
// {
//     public async Task Handle(StreamCreated domainEvent, CancellationToken cancellationToken)
//     {
//         var stream = domainEvent.Stream;
//
//         await streamerDbContext.SaveChangesAsync(cancellationToken);
//     }
// }
