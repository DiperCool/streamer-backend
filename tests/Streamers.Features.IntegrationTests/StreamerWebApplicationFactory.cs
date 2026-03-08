using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Hangfire;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Npgsql;
using NSubstitute;
using Respawn;
using Respawn.Graph;
using Shared.Seeds;
using Shared.Stripe;
using StackExchange.Redis;
using streamer.ServiceDefaults;
using streamer.ServiceDefaults.Identity;
using Streamers.Features.Chats.Services;
using Streamers.Features.Roles.Services;
using Streamers.Features.Shared.Persistance;
using Shared.Storage;
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

    public IStripeService StripeService { get; }

    public StreamerWebApplicationFactory()
    {
        StripeService = Substitute.For<IStripeService>();
        StripeService
            .CreateExpressAccountAsync(default!, default!, default)
            .ReturnsForAnyArgs(new CreateStripeAccountResult("acct_12345"));

        StripeService
            .CreateCustomerAsync(default!, default!, default)
            .ReturnsForAnyArgs("cus_12345");

        StripeService
            .CreateAccountLinkAsync(default!, default)
            .ReturnsForAnyArgs(("https://stripe.com/onboard", DateTime.UtcNow.AddHours(1)));

        StripeService
            .CreateSetupIntentAsync(default!, default)
            .ReturnsForAnyArgs("seti_12345_secret_67890");

        StripeService.DetachPaymentMethodAsync(default!, default!, default).ReturnsForAnyArgs(true);

        StripeService
            .UpdateCustomerDefaultPaymentMethodAsync(default!, default!, default)
            .ReturnsForAnyArgs(true);

        StripeService
            .CreatePaymentIntentAsync(
                default,
                default!,
                default!,
                default,
                default,
                default,
                default
            )
            .ReturnsForAnyArgs(new StripePaymentIntentResponse("pi_12345_secret_67890"));

        StripeService
            .CreateSubscriptionAsync(
                default!,
                default!,
                default,
                default,
                default,
                default,
                default!
            )
            .ReturnsForAnyArgs(
                new CreateSubscriptionResponse("sub_12345_secret_67890", "sub_12345")
            );

        StripeService.GetCurrentBalanceAsync(default, default).ReturnsForAnyArgs(100.0m);

        BackgroundJobClient = Substitute.For<IBackgroundJobClient>();
        MockTopicEventSender = Substitute.For<ITopicEventSender>();
        MockStorage = Substitute.For<IStorage>();

    }

    public IBackgroundJobClient BackgroundJobClient { get; }
    public ITopicEventSender MockTopicEventSender { get; }
    public IStorage MockStorage { get; }

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
            services.Replace(new ServiceDescriptor(typeof(IStripeService), StripeService));
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
            // Remove Hangfire hosted service
            var hangfireHostedServiceDescriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(IHostedService)
                && d.ImplementationType == typeof(BackgroundJobServerHostedService)
            );
            if (hangfireHostedServiceDescriptor != null)
            {
                services.Remove(hangfireHostedServiceDescriptor);
            }

            services.RemoveAll<IBackgroundJobClient>();
            services.AddSingleton(BackgroundJobClient);

            services.RemoveAll<ITopicEventSender>();
            services.AddSingleton(MockTopicEventSender);

            services.RemoveAll<IStorage>();
            services.AddSingleton(MockStorage);


        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();

        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StreamerDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public async Task Respawn()
    {
        await using var connection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        await connection.OpenAsync();

        var respawner = await Respawner.CreateAsync(
            connection,
            new RespawnerOptions
            {
                SchemasToInclude = ["public"],
                TablesToIgnore = new Table[] { "__EFMigrationsHistory", "spatial_ref_sys" },
                DbAdapter = DbAdapter.Postgres,
            }
        );
        await respawner.ResetAsync(connection);

        var redisConnection = Services.GetRequiredService<IConnectionMultiplexer>();
        var endpoints = redisConnection.GetEndPoints();

        var server = redisConnection.GetServer(endpoints.First());

        await server.FlushAllDatabasesAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _redisContainer.StopAsync();
        await base.DisposeAsync();
    }
}
