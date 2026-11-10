namespace Momentum.Contracts.DTOs;

public sealed record MoodDto(
    int MoodId,
    string Name,
    string? Emoji,
    byte Score,
    string ColorHex
);