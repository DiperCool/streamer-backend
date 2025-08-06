using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Domain;

namespace Shared.DomainEvents;

public static class Extensions
{
    public static IServiceCollection AddDomainEvents(
        this IServiceCollection services
    )
    {
        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();   
        return services;
    }
}

