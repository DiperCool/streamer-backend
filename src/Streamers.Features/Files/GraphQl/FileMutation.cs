using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using Shared.Abstractions.Cqrs;
using Streamers.Features.Files.Features;

namespace Streamers.Features.Files.GraphQl;

[MutationType]
public static partial class FileMutation
{
    [AllowAnonymous]
    public static async Task<UploadFileResponse> UploadAsync(
        UploadFile input,
        [Service] IMediator mediator
    )
    {
        var response = await mediator.Send(input);
        return response;
    }
}
