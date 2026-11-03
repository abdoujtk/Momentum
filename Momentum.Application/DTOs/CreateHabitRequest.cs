namespace Momentum.Application.DTOs;

public sealed record CreateHabitRequest(
    string Title,
    string? Description,
    decimal? TargetValue,
    string? TargetUnit,
    string FrequencyType,
    string? DaysOfWeek,
    string? ColorHex,
    string? IconName,
    int CategoryId
);