using Microsoft.Extensions.DependencyInjection;

namespace Shared.Abstractions.Cqrs;

public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    // Send - For command/query operations
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        
        var handler = serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"No handler registered for {requestType.Name}");

        // Get pipeline behaviors
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<IRequest<TResponse>, TResponse>>();
        
        // Create the request pipeline
        RequestHandlerDelegate<TResponse> pipeline = () =>
        {
            var method = handlerType.GetMethod("Handle");
            var result = method?.Invoke(handler, [request, cancellationToken]);            
            if (result is null)
                throw new InvalidOperationException("Handler returned null");

            return (Task<TResponse>)result;        };

        foreach (var behavior in behaviors.Reverse())
        {
            var currentPipeline = pipeline;
            pipeline = () => behavior.Handle(request, currentPipeline, cancellationToken);
        }

        return await pipeline();
    }
}
