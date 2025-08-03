using streamer.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddForwardedHeaders();

var redis = builder.AddRedis("redis");

var username = builder.AddParameter("rabbitName");
var rabbitMq = builder.AddRabbitMQ("eventbus", username, port: 5672)
    .WithManagementPlugin(port: 15672)
    .WithLifetime(ContainerLifetime.Persistent);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgWeb()
    .WithLifetime(ContainerLifetime.Persistent);

var streamerDb = postgres.AddDatabase("streamerdb");
var launchProfileName = ShouldUseHttpForEndpoints() ? "http" : "https";
var rabit = rabbitMq.GetEndpoint("https");
var keycloak = builder
    .AddKeycloak("keycloak", 8080)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithRealmImport("./Keycloak")
    .WithDataVolume()
    .WithEnvironment("KK_TO_RMQ_URL", "eventbus")
    .WithEnvironment("KK_TO_RMQ_PASSWORD", rabbitMq.Resource.PasswordParameter.Value)
    .WithEnvironment("KK_TO_RMQ_USERNAME", rabbitMq.Resource.UserNameParameter?.Value)
    .WithEnvironment("KK_TO_RMQ_VHOST", "/")
    .WithBindMount("./Keycloak","/opt/keycloak/providers");

var identityEndpoint = keycloak.GetEndpoint(launchProfileName);

var streamerApi = builder.AddProject<Projects.Streamers_Api>("streamer-api")
    .WithReference(redis).WaitFor(redis)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(keycloak).WaitFor(keycloak)
    .WithReference(streamerDb).WaitFor(streamerDb)
    .WithEnvironment("EventBusOptions__Host", "localhost")
    .WithEnvironment("EventBusOptions__Username", rabbitMq.Resource.UserNameParameter?.Value)
    .WithEnvironment("EventBusOptions__Password", rabbitMq.Resource.PasswordParameter.Value);


builder.Build().Run();

// For test use only.
// Looks for an environment variable that forces the use of HTTP for all the endpoints. We
// are doing this for ease of running the Playwright tests in CI.
static bool ShouldUseHttpForEndpoints()
{
    const string EnvVarName = "ESHOP_USE_HTTP_ENDPOINTS";
    var envValue = Environment.GetEnvironmentVariable(EnvVarName);

    // Attempt to parse the environment variable value; return true if it's exactly "1".
    return int.TryParse(envValue, out int result) && result == 1;
}
