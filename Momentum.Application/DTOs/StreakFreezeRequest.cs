namespace Momentum.Application.DTOs;

public sealed record StreakFreezeRequest(
    DateOnly FreezeDate,
    string? Reason
);