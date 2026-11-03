namespace Momentum.Application.DTOs;

public sealed record CheckInRequest(
    decimal? ActualValue,
    bool IsCompleted,
    int? MoodId,
    string? Note
);