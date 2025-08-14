using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Streams.Dtos;
using Streamers.Features.Streams.Features.GetCurrentStream;
using Streamers.Features.Streams.Features.GetStreamSettings;
using Streamers.Features.Streams.Features.UpdateStreamSettings;

namespace Streamers.Features.Streams.GraphQl;

[QueryType]
public static partial class StreamQuery
{
    [Authorize]
    public static async Task<StreamSettingsDto> GetStreamSettings(IMediator mediator)
    {
        return await mediator.Send(new GetStreamSettings());
    }

    public static async Task<StreamDto> GetCurrentStream(string streamerId, IMediator mediator)
    {
        return await mediator.Send(new GetCurrentStream(streamerId));
    }
}
