namespace Momentum.Contracts.DTOs;
 public sealed record StreakFreezeResultDto(
    int FreezeId,
    int HabitId,
    DateOnly FreezeDate,
    string? Reason,
    int RemainingFreezesThisMonth
);

