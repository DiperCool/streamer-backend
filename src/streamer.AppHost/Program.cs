using streamer.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddForwardedHeaders();
var redis = builder.AddRedis("redis");

var postgres = builder
    .AddPostgres("postgres")
    .WithDataVolume()
    .WithPgWeb()
    .WithLifetime(ContainerLifetime.Persistent);

var streamerDb = postgres.AddDatabase("streamerdb");

var minio = builder
    .AddContainer("minio", "minio/minio")
    .WithArgs("server", "/data", "--console-address", ":9001")
    .WithEnvironment("MINIO_ROOT_USER", "minioadmin")
    .WithEnvironment("MINIO_ROOT_PASSWORD", "minioadmin")
    .WithBindMount("./minio-data", "/data")
    .WithHttpEndpoint(targetPort: 9000, name: "api", isProxied: false) // expose MinIO API port
    .WithHttpEndpoint(targetPort: 9001, name: "console", isProxied: false); // expose MinIO console port

var streamerApi = builder
    .AddProject<Projects.Streamers_Api>("streamer-api")
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(streamerDb)
    .WaitFor(streamerDb)
    .WaitFor(minio);
var mediamtx = builder
    .AddContainer("mediamtx", "dipercool/mediamtx-curl")
    .WithBindMount("./rtsp-server/mediamtx.yml", "/mediamtx.yml")
    .WaitFor(streamerApi)
    .WithEnvironment(
        "RABBITMQ_URI",
        "amqps://ycxdqxek:dqbA9K8DP4m6IPTboHah9UD8ZTPnM6Qb@kebnekaise.lmq.cloudamqp.com/ycxdqxek"
    )
    .WithEndpoint(targetPort: 1935, scheme: "rtmp", isProxied: false)
    .WithHttpEndpoint(targetPort: 8888, name: "hls", isProxied: false)
    .WithHttpEndpoint(targetPort: 8889, name: "webrtc", isProxied: false)
    .WithHttpEndpoint(targetPort: 9997, name: "api", isProxied: false);
builder.Build().Run();
