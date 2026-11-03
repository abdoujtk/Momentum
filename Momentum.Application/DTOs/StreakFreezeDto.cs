namespace Momentum.Application.DTOs;

public sealed record StreakFreezeDto(
    int FreezeId,
    int HabitId,
    DateOnly FreezeDate,
    string? Reason,
    int RemainingFreezesThisMonth
);