using System.Net.Http.Json;
using Momentum.Contracts.DTOs.Auth;
using Momentum.Desktop.ViewModels.Abstractions;

namespace Momentum.Client.Http;

public class AuthService : IAuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http) => _http = http;

    public async Task<AuthResponse?> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", new LoginRequest(email, password), ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ct);
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/auth/register", request, ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: ct);
    }
}