using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Streams.Features.UpdateStreamSettings;

namespace Streamers.Features.Streams.GraphQl;

[MutationType]
public static partial class StreamMutation
{
    [Authorize]
    public static async Task<UpdateStreamSettingsResponse> UpdateStreamSettings(IMediator mediator)
    {
        return await mediator.Send(new UpdateStreamSettings());
    }
}
