using System.Security.Claims;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Http;

namespace Streamers.Features.Shared.GraphQl;

// TODO: fix anonymous accessing
public class AnonymousRequestInterceptor : DefaultHttpRequestInterceptor
{
    public override ValueTask OnCreateAsync(
        HttpContext context,
        IRequestExecutor requestExecutor,
        OperationRequestBuilder requestBuilder,
        CancellationToken cancellationToken
    )
    {
        if (context.User.Identities.All(t => !t.IsAuthenticated))
        {
            context.User.AddIdentity(new ClaimsIdentity("app-anonymous"));
        }

        return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
    }
}
