namespace Momentum.Contracts.DTOs.Auth;

public sealed record AuthResponse(
    string Token,
    string DisplayName,
    string Email
);