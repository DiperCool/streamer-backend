using System.Data.Common;
using DotNet.Testcontainers.Builders;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Shared.Seeds;
using StackExchange.Redis;
using streamer.ServiceDefaults;
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
    private Respawner _respawner = null!;
    private DbConnection _connection = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.UseSetting(
            "ConnectionStrings:redis",
            $"{_redisContainer.GetConnectionString()},allowAdmin=true"
        );
        builder.UseSetting("ConnectionStrings:streamerdb", _dbContainer.GetConnectionString());

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDataSeeder>();

            services.RemoveAll<ICurrentUser>();

            services.AddScoped<ICurrentUser, StubCurrentUser>();
            services.RemoveAll<CapOptions>();
            services.AddCap(x =>
            {
                x.UseEntityFramework<StreamerDbContext>();

                x.UseInMemoryStorage();
            });
            var migrationServiceDescriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(IHostedService)
                && d.ImplementationType == typeof(MigrationWorker<StreamerDbContext>)
            );
            if (migrationServiceDescriptor != null)
            {
                services.Remove(migrationServiceDescriptor);
            }
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StreamerDbContext>();
        await dbContext.Database.MigrateAsync();

        await InitRespawn();
    }

    private async Task InitRespawn()
    {
        _connection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await _connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            _connection,
            new RespawnerOptions
            {
                SchemasToInclude = ["public"],
                TablesToIgnore = new Table[] { "__EFMigrationsHistory", "spatial_ref_sys" },
                DbAdapter = DbAdapter.Postgres,
            }
        );
    }

    public async Task Respawn()
    {
        await _respawner.ResetAsync(_connection);
        var redisConnection = Services.GetRequiredService<IConnectionMultiplexer>();
        var endpoints = redisConnection.GetEndPoints();

        var server = redisConnection.GetServer(endpoints.First());

        await server.FlushAllDatabasesAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
        await base.DisposeAsync();
    }
}
