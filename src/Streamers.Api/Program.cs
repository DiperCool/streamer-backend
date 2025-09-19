using System.Web;
using Microsoft.AspNetCore.Mvc;
using Shared.Abstractions.Cqrs;
using streamer.ServiceDefaults;
using Streamers.Api.Apis;
using Streamers.Features;
using Streamers.Features.AntMedia.Models;
using Streamers.Features.Streams.Features.CanStartStream;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddProblemDetails();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

var app = builder.Build();
app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseWebSockets();

app.MapDefaultEndpoints();

app.NewVersionedApi("Streamers").MapOrdersApiV1().RequireAuthorization();
app.UseDefaultOpenApi();

app.MapPost(
    "/rtmp/checkToken",
    async (HttpRequest request, [FromBody] StreamToken body, IMediator mediator) =>
    {
        if (string.IsNullOrEmpty(body.Query))
        {
            return Results.Unauthorized();
        }

        var queryCollection = HttpUtility.ParseQueryString(body.Query);
        var token = queryCollection["token"] ?? string.Empty;
        var response = await mediator.Send(
            new CanStartStream(token, body.Path, body.Action, body.Protocol)
        );
        return response.Response ? Results.Ok() : Results.Unauthorized();
    }
);

app.MapGraphQL();
app.Run();
