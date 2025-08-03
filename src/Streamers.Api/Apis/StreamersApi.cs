using Microsoft.AspNetCore.Http.HttpResults;

namespace Streamers.Api.Apis;

public static class StreamersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders").HasApiVersion(1.0);

        api.MapPut("/test", GetOrderAsync);

        return api;
    }
    public static Task<Results<Ok<string>, NotFound>> GetOrderAsync()
    {
        return Task.FromResult<Results<Ok<string>, NotFound>>(TypedResults.Ok("s"));
    }
}
