using FluentValidation;
using MediatR;
using Momentum.Application.Common;
using System.Runtime.CompilerServices;

namespace Momentum.Application.Common.Behaviors;


public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
  

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var message = string.Join(" | ", failures.Select(f => f.ErrorMessage));
        return CreateFailure(Error.Validation(message));
    }

    private static TResponse CreateFailure(Error error)
    {
        
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(error);

        var valueType = typeof(TResponse).GetGenericArguments()[0];
        var failureMethod = typeof(Result<>).MakeGenericType(valueType)
            .GetMethod(nameof(Result<object>.Failure))!;
        return (TResponse)failureMethod.Invoke(null, [error])!;
    }

    
}