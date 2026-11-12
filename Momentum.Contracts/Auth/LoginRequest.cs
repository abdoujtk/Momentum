namespace Momentum.Contracts.DTOs.Auth;

public sealed record LoginRequest(
    string Email,
    string Password
);