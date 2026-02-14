using W2K.Common.Extensions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace W2K.Common.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger = logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var optionalValidation = request as IOptionalValidation;
        if (optionalValidation?.IsValidationDisabled == true)
        {
            return await next(cancellationToken);
        }
        var typeName = request.GetGenericTypeName();

        var failures = new List<ValidationFailure>();
        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, cancellationToken);
            failures.AddRange(result.Errors);
        }

        // Suppress Sonar warning: S2583 - Condition is always true or false
#pragma warning disable S2583
        if (failures.Count != 0)
        {
#pragma warning restore S2583
            _logger.LogWarning("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", typeName, request, failures);
            throw new ValidationException($"Command Validation Errors for type {typeof(TRequest).Name}", failures);
        }

        return await next(cancellationToken);
    }
}
