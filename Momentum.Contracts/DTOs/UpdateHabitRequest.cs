namespace Momentum.Contracts.DTOs;

public sealed record UpdateHabitRequest(
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