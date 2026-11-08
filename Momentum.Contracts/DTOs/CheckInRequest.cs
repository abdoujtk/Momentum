namespace Momentum.Contracts.DTOs;

public sealed record CheckInRequest(
    decimal? ActualValue,
    bool IsCompleted,
    int? MoodId,
    string? Note
);