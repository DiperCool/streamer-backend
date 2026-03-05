using DotNetCore.CAP;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Seeds;
using StackExchange.Redis;
using streamer.ServiceDefaults.Identity;
using Streamers.Api;
using Streamers.Features.Shared.Persistance;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Streamers.Features.IntegrationTests;

public class StreamerWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16.3-alpine")
        .WithDatabase("streamer")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.2.5-alpine")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration(
            (context, conf) =>
            {
                conf.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        { "ConnectionStrings:streamerdb", _dbContainer.GetConnectionString() },
                        { "ConnectionStrings:redis", _redisContainer.GetConnectionString() },
                    }
                );
            }
        );

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<StreamerDbContext>));

            services.AddDbContext<StreamerDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString())
            );

            services.RemoveAll<IConnectionMultiplexer>();
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(_redisContainer.GetConnectionString())
            );
            services.RemoveAll<IDataSeeder>();

            services.RemoveAll<ICurrentUser>();

            services.AddScoped<ICurrentUser, StubCurrentUser>();
            services.RemoveAll<CapOptions>();
            services.AddCap(x =>
            {
                x.UseEntityFramework<StreamerDbContext>();

                x.UseInMemoryStorage();
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
    }
}
