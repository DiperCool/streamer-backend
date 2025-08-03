using streamer.ServiceDefaults;
using Streamers.Api.Apis;
using Streamers.Api.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();
builder.Services.AddProblemDetails();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

var app = builder.Build();

app.MapDefaultEndpoints();

app.NewVersionedApi("Streamers").MapOrdersApiV1().RequireAuthorization();
app.UseDefaultOpenApi();
app.Run();
