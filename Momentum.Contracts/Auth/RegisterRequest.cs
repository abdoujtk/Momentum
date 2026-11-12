namespace Momentum.Contracts.DTOs.Auth;

public sealed record RegisterRequest(
    string DisplayName,
    string Email,
    string Password,
    string TimeZoneId = "Asia/Amman"
);