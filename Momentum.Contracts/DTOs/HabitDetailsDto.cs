namespace Momentum.Contracts.DTOs;

public sealed record HabitDetailsDto(
    HabitDto Habit,
    int CurrentStreak,
    int BestStreak,
    DateOnly? LastCompletedDate,
    int TotalCompletedDays,
    int TotalFrozenDays,
    IReadOnlyList<CheckInDto> RecentCheckIns,
    IReadOnlyList<StreakFreezeDto> RecentFreezes
);