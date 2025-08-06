using streamer.ServiceDefaults;
using Streamers.Api.Apis;
using Streamers.Features.Shared.Extensions;

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
app.MapDefaultEndpoints();

app.NewVersionedApi("Streamers").MapOrdersApiV1().RequireAuthorization();
app.UseDefaultOpenApi();
app.MapGraphQL();

app.Run();
