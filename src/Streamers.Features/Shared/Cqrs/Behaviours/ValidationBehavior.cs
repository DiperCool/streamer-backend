using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shared.Abstractions.Cqrs;

namespace Streamers.Features.Shared.Cqrs.Behaviours;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider =
            serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestType = request.GetType();
        var validatorType = typeof(IValidator<>).MakeGenericType(requestType);

        var validators = _serviceProvider.GetServices(validatorType).Cast<IValidator>();

        if (!validators.Any())
            return await next();

        var contextType = typeof(ValidationContext<>).MakeGenericType(requestType);
        var context = Activator.CreateInstance(contextType, request);

        var validationTasks = validators.Select(v =>
        {
            var method = v.GetType()
                .GetMethod("ValidateAsync", [contextType, typeof(CancellationToken)]);
            return (Task<FluentValidation.Results.ValidationResult>)
                method!.Invoke(v, [context!, cancellationToken])!;
        });

        var validationResults = await Task.WhenAll(validationTasks);

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count > 0)
            throw new ValidationException(failures);

        return await next();
    }
}

/// <summary>
/// Custom validation exception for FluentValidation errors
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        : base("One or more validation failures have occurred.")
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}
