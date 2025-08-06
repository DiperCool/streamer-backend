using System.Globalization;
using DotNetCore.CAP.Internal;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Shared.RabbitMQ.Extensions;

public static class Extensions
{
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        IConfigurationSection rabbitMqSection = configuration.GetSection("RabbitMQ");
        string host = rabbitMqSection.GetValue<string>("HostName") ?? "localhost";
        int port = rabbitMqSection.GetValue<int?>("Port") ?? 5672;
        string user = rabbitMqSection.GetValue<string>("UserName") ?? "guest";
        string pass = rabbitMqSection.GetValue<string>("Password") ?? "guest";
        string uri = rabbitMqSection.GetValue<string>("Uri") ?? "";
        services.AddCap(x =>
        {
            x.UsePostgreSql(o =>
            {
                o.DataSource = NpgsqlDataSource.Create(
                    configuration.GetConnectionString("streamerdb") ?? ""
                );
            });
            x.UseRabbitMQ(cfg =>
            {
                cfg.CustomHeadersBuilder = (msg, sp) =>

                [
                    new(
                        DotNetCore.CAP.Messages.Headers.MessageId,
                        sp.GetRequiredService<ISnowflakeId>()
                            .NextId()
                            .ToString(CultureInfo.InvariantCulture)
                    ),
                    new(DotNetCore.CAP.Messages.Headers.MessageName, msg.RoutingKey),
                ];
                cfg.HostName = host;
                cfg.Port = port;
                cfg.UserName = user;
                cfg.Password = pass;
                cfg.VirtualHost = user;
                cfg.ConnectionFactoryOptions = factory =>
                {
                    factory.Uri = new Uri(uri);
                };
            });
        });
        return services;
    }
}
