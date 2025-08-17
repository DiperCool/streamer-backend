using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Domain;

namespace Shared.DomainEvents;

public static class Extensions
{
    public static IServiceCollection AddDomainEvents(
        this IServiceCollection services,
        Assembly assembly
    )
    {
        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();
        RegisterHandlers(services, assembly);
        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        var requestHandlerTypes = assembly
            .GetTypes()
            .Where(t =>
                t.GetInterfaces()
                    .Any(i =>
                        i.IsGenericType
                        && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
                    )
            )
            .ToList();

        foreach (var handlerType in requestHandlerTypes)
        {
            var handlerInterface = handlerType
                .GetInterfaces()
                .First(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>)
                );

            services.AddTransient(handlerInterface, handlerType);
        }
    }
}
