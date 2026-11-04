namespace Momentum.Application.DTOs;

public sealed record HeatmapDto(
    DateOnly StartDate,
    DateOnly EndDate,
    int TotalScheduled,
    int TotalCompleted,
    decimal OverallCompletionPercent,
    IReadOnlyList<HeatmapDayDto> Days
);
public sealed record HeatmapDayDto(
    DateOnly Date,
    int DayOfWeek,
    int ScheduledHabits,
    int CompletedHabits,
    int FrozenHabits,
    decimal CompletionPercent,
    int Level  // 0..4
);

