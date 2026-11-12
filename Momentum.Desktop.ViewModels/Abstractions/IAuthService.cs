using Momentum.Contracts.DTOs.Auth;

namespace Momentum.Desktop.ViewModels.Abstractions;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(string email, string password, CancellationToken ct = default);
    Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
}