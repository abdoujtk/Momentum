namespace Momentum.Desktop.ViewModels.Abstractions;

public interface ISecureTokenStorage
{
    Task SaveTokenAsync(string token, string displayName);
    Task<(string? Token, string? DisplayName)> GetTokenAsync();
    void ClearToken();
}