using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Abstractions.Cqrs;

public static class MediatorExtensions
{
    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        Assembly assembly
    )
    {
        services.AddScoped<IMediator, Mediator>();
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
                        && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                    )
            )
            .ToList();

        foreach (var handlerType in requestHandlerTypes)
        {
            var handlerInterface = handlerType
                .GetInterfaces()
                .First(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                );

            services.AddTransient(handlerInterface, handlerType);
        }
    }
}
