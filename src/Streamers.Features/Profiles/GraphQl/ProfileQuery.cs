using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Profiles.Dtos;
using Streamers.Features.Profiles.Features.GetProfile;
using Streamers.Features.Profiles.Models;

namespace Streamers.Features.Profiles.GraphQl;

[QueryType]
public class ProfileQuery
{
    public async Task<ProfileDto> GetProfile(string streamerId, IMediator mediator)
    {
        return await mediator.Send(new GetProfile(streamerId));
    }
}
