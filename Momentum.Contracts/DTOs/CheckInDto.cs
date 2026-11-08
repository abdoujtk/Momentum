namespace Momentum.Contracts.DTOs;

public sealed record CheckInDto(
    int CheckInId,
    int HabitId,
    DateOnly CheckInDate,
    decimal? ActualValue,
    bool IsCompleted,
    int? MoodId,
    string? MoodName,
    string? MoodEmoji,
    string? Note
);