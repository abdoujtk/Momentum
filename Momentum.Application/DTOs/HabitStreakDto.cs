namespace Momentum.Application.DTOs;

public sealed record HabitStreakDto(
    int HabitId,
    string Title,
    string CategoryName,
    int CurrentStreak,
    int BestStreak,
    DateOnly? LastCompletedDate,
    int TotalCompletedDays,
    int TotalFrozenDays
);