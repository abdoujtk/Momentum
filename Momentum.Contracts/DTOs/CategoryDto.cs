namespace Momentum.Contracts.DTOs;

public sealed record CategoryDto(
    int CategoryId,
    string Name,
    string ColorHex
);