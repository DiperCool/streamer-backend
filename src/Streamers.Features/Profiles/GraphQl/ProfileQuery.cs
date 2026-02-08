using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Profiles.Dtos;
using Streamers.Features.Profiles.Features.GetProfile;

namespace Streamers.Features.Profiles.GraphQl;

[QueryType]
public class ProfileQuery
{
    public async Task<ProfileDto> GetProfile(string streamerId, IMediator mediator)
    {
        return await mediator.Send(new GetProfile(streamerId));
    }
}
