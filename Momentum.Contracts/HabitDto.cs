namespace Momentum.Contracts;

public sealed record HabitDto(
    int HabitId,
    string Title,
    string? Description,
    decimal? TargetValue,
    string? TargetUnit,
    string FrequencyType,
    string? DaysOfWeek,
    string? ColorHex,
    string? IconName,
    int CategoryId,
    string CategoryName,
    string CategoryColorHex,
    bool IsCompletedToday,
    decimal? TodayActualValue,
    bool IsScheduledToday
    );