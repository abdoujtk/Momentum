namespace Momentum.Contracts.DTOs;

public sealed record StreakFreezeRequest(
    DateOnly FreezeDate,
    string? Reason
);