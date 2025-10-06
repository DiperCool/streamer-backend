using streamer.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddForwardedHeaders();
var redis = builder.AddRedis("redis");

var postgres = builder
    .AddPostgres("postgres")
    .WithDataVolume()
    .WithPgWeb()
    .WithLifetime(ContainerLifetime.Persistent);
var mongo = builder.AddMongoDB("mongo").WithDataVolume().WithLifetime(ContainerLifetime.Persistent);

var mongodb = mongo.AddDatabase("streamerBots");

var streamerDb = postgres.AddDatabase("streamerdb");
var rabbitMqUri = builder.AddParameter("rabbitmqUri");

var s3AccessKey = builder.AddParameter("s3accesskey");
var s3SecretKey = builder.AddParameter("s3secretkey");
var s3Bucket = builder.AddParameter("s3bucket");
var s3Region = builder.AddParameter("s3region");

var streamerApi = builder
    .AddProject<Projects.Streamers_Api>("streamer-api")
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(streamerDb)
    .WaitFor(streamerDb);
var mediamtx = builder
    .AddContainer("mediamtx", "dipercool/mediamtx-curl")
    .WithBindMount("./rtsp-server/mediamtx.yml", "/mediamtx.yml")
    .WaitFor(streamerApi)
    .WithEnvironment("RABBITMQ_URI", rabbitMqUri)
    .WithEndpoint(targetPort: 1935, scheme: "rtmp", isProxied: false)
    .WithEndpoint(targetPort: 8554, scheme: "rtsp", isProxied: false)
    .WithHttpEndpoint(targetPort: 8888, name: "hls", isProxied: false)
    .WithHttpEndpoint(targetPort: 8889, name: "webrtc", isProxied: false)
    .WithHttpEndpoint(targetPort: 9997, name: "api", isProxied: false);

var vodProcessor = builder
    .AddContainer("vod-processor", "dipercool/vod-processor")
    .WaitFor(streamerApi)
    .WaitFor(mediamtx)
    .WithEnvironment("RABBIT_URL", rabbitMqUri)
    .WithEnvironment("AWS_SECRET_ACCESS_KEY", s3SecretKey)
    .WithEnvironment("AWS_ACCESS_KEY_ID", s3AccessKey)
    .WithEnvironment("AWS_REGION", s3Region)
    .WithEnvironment("AWS_BUCKET", s3Bucket);
var streamerBots = builder
    .AddContainer("streamer-bots", "dipercool/streamer-bots")
    .WaitFor(streamerApi)
    .WaitFor(mediamtx)
    .WithEnvironment("RABBITMQ_URI", rabbitMqUri)
    .WithEnvironment("RTMP_BASE_URL", "rtmp://mediamtx:1935/live")
    .WithEnvironment("MONGODB_URI", mongodb);
builder.Build().Run();
