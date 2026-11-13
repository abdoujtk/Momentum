namespace Momentum.Desktop.ViewModels.Abstractions;

public interface ITokenStore
{
    string? Token { get; }
    string? DisplayName { get; }

    Task SetTokenAsync(string token, string displayName);
    Task LoadTokenAsync();
    void Clear();
}