namespace Momentum.Application.Common;


public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error NotFound(string entity)
        => new($"{entity}.NotFound", $"{entity} not found.");

    public static Error Validation(string message)
        => new("Validation", message);

    public static Error Unauthorized()
        => new("Unauthorized", "User is not authorized.");

    public static Error Unexpected(string message)
        => new("Unexpected", message);
}